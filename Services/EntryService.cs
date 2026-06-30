using Microsoft.EntityFrameworkCore;
using Pravesh.API.Data;
using Pravesh.API.DTOs.Entry;
using Pravesh.API.Entities;
using Pravesh.API.Enums;
using Pravesh.API.Services.Interfaces;

namespace Pravesh.API.Services;

public class EntryService : IEntryService
{
    private readonly AppDbContext _db;
    private readonly IEmailService _email;

    public EntryService(AppDbContext db, IEmailService email)
    {
        _db    = db;
        _email = email;
    }

    // ── ConfirmEntry ──────────────────────────────────────────────────────────
    public async Task<object> ConfirmEntryAsync(int guardId, ConfirmEntryRequest req)
    {
        var now = DateTime.Now;

        var gate = await _db.Gates
            .FirstOrDefaultAsync(g => g.AssignedGuardId == guardId && g.IsActive);

        var pass = await _db.VisitorPasses
            .Include(p => p.Flat)
            .Include(p => p.Resident)
            .FirstOrDefaultAsync(p => p.Uuid == req.Uuid);

        if (pass == null)
            return new
            {
                result  = "DENIED",
                reason  = "PASS_NOT_FOUND",
                message = "No pass found with this UUID."
            };

        ScanResult  scanResult = ScanResult.GRANTED;
        DenyReason? denyReason = null;

        if (pass.Status == PassStatus.REVOKED)
        {
            scanResult = ScanResult.DENIED;
            denyReason = DenyReason.REVOKED;
        }
        else if (pass.Status == PassStatus.CONSUMED)
        {
            scanResult = ScanResult.DENIED;
            denyReason = DenyReason.ALREADY_USED;
        }
        else if (pass.Status == PassStatus.EXPIRED || pass.ValidUntil < now)
        {
            scanResult = ScanResult.DENIED;
            denyReason = DenyReason.QR_EXPIRED;
        }
        else if (pass.ValidFrom > now)
        {
            scanResult = ScanResult.DENIED;
            denyReason = DenyReason.NOT_YET_ACTIVE;
        }

        if (scanResult == ScanResult.GRANTED)
        {
            if (pass.PassType == PassType.ONE_TIME)
            {
                pass.Status        = PassStatus.CONSUMED;
                pass.UsesRemaining = 0;
            }
            else if (pass.PassType == PassType.MULTI_USE)
            {
                pass.UsesRemaining--;
                if (pass.UsesRemaining <= 0)
                    pass.Status = PassStatus.CONSUMED;
            }
        }

        var entryLog = new EntryLog
        {
            PassId      = pass.Id,
            UuidScanned = req.Uuid,
            GateId      = gate?.Id,
            GuardId     = guardId,
            ScanResult  = scanResult,
            DenyReason  = denyReason,
            ScannedAt   = now
        };

        _db.EntryLogs.Add(entryLog);
        await _db.SaveChangesAsync();

        if (scanResult == ScanResult.GRANTED)
        {
            try
            {
                await _email.SendEntryAlertEmailAsync(
                    pass.Resident.Email,
                    pass.Resident.Name,
                    pass.VisitorName,
                    gate?.Name ?? "Unknown Gate",
                    now);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Email Error] {ex.Message}");
            }
        }

        return new
        {
            result      = scanResult.ToString(),
            reason      = denyReason?.ToString(),
            visitorName = pass.VisitorName,
            flatNumber  = pass.Flat.FlatNumber,
            gate        = gate?.Name ?? "Unknown",
            scannedAt   = now
        };
    }

    // ── GetMyGateEntries ──────────────────────────────────────────────────────
    public async Task<List<EntryLogResponse>> GetMyGateEntriesAsync(int guardId)
    {
        var today = DateTime.Now.Date;

        var logs = await _db.EntryLogs
            .Include(e => e.VisitorPass).ThenInclude(p => p.Flat)
            .Include(e => e.Gate)
            .Include(e => e.Guard)
            .Where(e => e.GuardId == guardId && e.ScannedAt.Date == today)
            .OrderByDescending(e => e.ScannedAt)
            .ToListAsync();

        return logs.Select(MapToResponse).ToList();
    }

    // ── GetFlatEntries ────────────────────────────────────────────────────────
    public async Task<List<EntryLogResponse>> GetFlatEntriesAsync(int flatId)
    {
        var logs = await _db.EntryLogs
            .Include(e => e.VisitorPass).ThenInclude(p => p.Flat)
            .Include(e => e.Gate)
            .Include(e => e.Guard)
            .Where(e => e.VisitorPass.FlatId == flatId)
            .OrderByDescending(e => e.ScannedAt)
            .ToListAsync();

        return logs.Select(MapToResponse).ToList();
    }

    // ── GetAllEntries ─────────────────────────────────────────────────────────
    public async Task<List<EntryLogResponse>> GetAllEntriesAsync(int? societyId)
    {
        var query = _db.EntryLogs
            .Include(e => e.VisitorPass).ThenInclude(p => p.Flat)
                .ThenInclude(f => f.Society)
            .Include(e => e.Gate)
            .Include(e => e.Guard)
            .AsQueryable();

        if (societyId.HasValue)
            query = query.Where(e => e.VisitorPass.Flat.SocietyId == societyId);

        var logs = await query
            .OrderByDescending(e => e.ScannedAt)
            .ToListAsync();

        return logs.Select(MapToResponse).ToList();
    }

    // ── Helper ────────────────────────────────────────────────────────────────
    private static EntryLogResponse MapToResponse(EntryLog e) => new()
    {
        Id               = e.Id,
        PassId           = e.PassId,
        UuidScanned      = e.UuidScanned,
        VisitorName      = e.VisitorPass?.VisitorName ?? "",
        FlatNumber       = e.VisitorPass?.Flat?.FlatNumber ?? "",
        GateId           = e.GateId,
        GateName         = e.Gate?.Name,
        GuardId          = e.GuardId,
        GuardName        = e.Guard?.Name,
        ScanResult       = e.ScanResult.ToString(),
        DenyReason       = e.DenyReason?.ToString(),
        ScannedAt        = e.ScannedAt,
        ResidentNotified = e.ResidentNotified
    };
}
