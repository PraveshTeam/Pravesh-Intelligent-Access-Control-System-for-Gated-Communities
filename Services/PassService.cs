using Microsoft.EntityFrameworkCore;
using Pravesh.API.Data;
using Pravesh.API.DTOs.Pass;
using Pravesh.API.Entities;
using Pravesh.API.Enums;
using Pravesh.API.Services.Interfaces;

namespace Pravesh.API.Services;

public class PassService : IPassService
{
    private readonly AppDbContext _db;
    private readonly IEmailService _email;

    public PassService(AppDbContext db, IEmailService email)
    {
        _db    = db;
        _email = email;
    }

    // ── CreatePass ────────────────────────────────────────────────────────────
    public async Task<PassResponse> CreatePassAsync(int userId, CreatePassRequest req)
    {
        var now = DateTime.Now;

        if (req.ValidFrom >= req.ValidUntil)
            throw new ArgumentException("ValidUntil must be after ValidFrom.");

        if (req.ValidUntil <= now)
            throw new ArgumentException("ValidUntil must be in the future.");

        if (req.PassType == PassType.MULTI_USE && req.UsesAllowed < 2)
            throw new ArgumentException("MULTI_USE pass must have at least 2 uses.");

        var resident = await _db.Users
            .Include(u => u.Flat)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (resident == null)
            throw new KeyNotFoundException("Resident not found.");

        if (resident.FlatId == null)
            throw new ArgumentException("You are not assigned to any flat yet.");

        var usesAllowed   = req.PassType == PassType.ONE_TIME ? 1 : req.UsesAllowed;
        var usesRemaining = usesAllowed;

        var pass = new VisitorPass
        {
            Uuid          = Guid.NewGuid().ToString(),
            FlatId        = resident.FlatId.Value,
            ResidentId    = userId,
            VisitorName   = req.VisitorName,
            VisitorPhone  = req.VisitorPhone,
            PassType      = req.PassType,
            UsesAllowed   = usesAllowed,
            UsesRemaining = usesRemaining,
            ValidFrom     = req.ValidFrom,
            ValidUntil    = req.ValidUntil,
            Status        = PassStatus.ACTIVE,
            CreatedAt     = now
        };

        _db.VisitorPasses.Add(pass);
        await _db.SaveChangesAsync();

        try
        {
            await _email.SendPassCreatedEmailAsync(
                resident.Email,
                resident.Name,
                pass.VisitorName,
                resident.Flat!.FlatNumber,
                pass.ValidFrom,
                pass.ValidUntil,
                pass.Uuid);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Email Error] {ex.Message}");
        }

        return MapToResponse(pass, resident.Flat!.FlatNumber, resident.Name);
    }

    // ── GetMyActivePasses ─────────────────────────────────────────────────────
    public async Task<List<PassResponse>> GetMyActivePassesAsync(int userId)
    {
        var passes = await _db.VisitorPasses
            .Include(p => p.Flat)
            .Include(p => p.Resident)
            .Where(p => p.ResidentId == userId && p.Status == PassStatus.ACTIVE)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();

        return passes
            .Select(p => MapToResponse(p, p.Flat.FlatNumber, p.Resident.Name))
            .ToList();
    }

    // ── GetMyPassHistory ──────────────────────────────────────────────────────
    public async Task<List<PassResponse>> GetMyPassHistoryAsync(int userId)
    {
        var passes = await _db.VisitorPasses
            .Include(p => p.Flat)
            .Include(p => p.Resident)
            .Where(p => p.ResidentId == userId)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();

        return passes
            .Select(p => MapToResponse(p, p.Flat.FlatNumber, p.Resident.Name))
            .ToList();
    }

    // ── GetPassById ───────────────────────────────────────────────────────────
    public async Task<PassResponse> GetPassByIdAsync(int userId, int passId)
    {
        var pass = await _db.VisitorPasses
            .Include(p => p.Flat)
            .Include(p => p.Resident)
            .Include(p => p.EntryLogs)
            .FirstOrDefaultAsync(p => p.Id == passId && p.ResidentId == userId);

        if (pass == null)
            throw new KeyNotFoundException("Pass not found.");

        return MapToResponse(pass, pass.Flat.FlatNumber, pass.Resident.Name);
    }

    // ── RevokePass ────────────────────────────────────────────────────────────
    public async Task RevokePassAsync(int userId, int passId)
    {
        var pass = await _db.VisitorPasses
            .FirstOrDefaultAsync(p => p.Id == passId && p.ResidentId == userId);

        if (pass == null)
            throw new KeyNotFoundException("Pass not found.");

        if (pass.Status != PassStatus.ACTIVE)
            throw new ArgumentException($"Cannot revoke a pass with status '{pass.Status}'.");

        pass.Status = PassStatus.REVOKED;
        await _db.SaveChangesAsync();
    }

    // ── Helper ────────────────────────────────────────────────────────────────
    private static PassResponse MapToResponse(
        VisitorPass pass, string flatNumber, string residentName) => new()
    {
        Id            = pass.Id,
        Uuid          = pass.Uuid,
        FlatId        = pass.FlatId,
        FlatNumber    = flatNumber,
        ResidentId    = pass.ResidentId,
        ResidentName  = residentName,
        VisitorName   = pass.VisitorName,
        VisitorPhone  = pass.VisitorPhone,
        PassType      = pass.PassType.ToString(),
        UsesAllowed   = pass.UsesAllowed,
        UsesRemaining = pass.UsesRemaining,
        ValidFrom     = pass.ValidFrom,
        ValidUntil    = pass.ValidUntil,
        Status        = pass.Status.ToString(),
        CreatedAt     = pass.CreatedAt
    };
}
