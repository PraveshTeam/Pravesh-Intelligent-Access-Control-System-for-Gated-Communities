using Microsoft.EntityFrameworkCore;
using Pravesh.API.Data;
using Pravesh.API.DTOs.Society;
using Pravesh.API.Entities;
using Pravesh.API.Enums;
using Pravesh.API.Services.Interfaces;

namespace Pravesh.API.Services;

public class GateService : IGateService
{
    private readonly AppDbContext _db;

    public GateService(AppDbContext db)
    {
        _db = db;
    }

    // ── AddGate ───────────────────────────────────────────────────────────────
    public async Task<GateResponse> AddGateAsync(int societyId, CreateGateRequest req)
    {
        var societyExists = await _db.Societies.AnyAsync(s => s.Id == societyId);
        if (!societyExists)
            throw new KeyNotFoundException("Society not found.");

        var gate = new Gate
        {
            SocietyId = societyId,
            Name      = req.Name,
            Location  = req.Location,
            IsActive  = true
        };

        _db.Gates.Add(gate);
        await _db.SaveChangesAsync();

        return new GateResponse
        {
            Id        = gate.Id,
            SocietyId = gate.SocietyId,
            Name      = gate.Name,
            Location  = gate.Location,
            IsActive  = gate.IsActive
        };
    }

    // ── AssignGuard ───────────────────────────────────────────────────────────
    public async Task<string> AssignGuardAsync(int gateId, AssignGuardRequest req)
    {
        var gate = await _db.Gates.FindAsync(gateId);
        if (gate == null)
            throw new KeyNotFoundException("Gate not found.");

        var guard = await _db.Users.FindAsync(req.GuardId);
        if (guard == null)
            throw new KeyNotFoundException("User not found.");

        if (guard.Role != UserRole.GUARD)
            throw new ArgumentException("User is not a GUARD.");

        if (guard.SocietyId != gate.SocietyId)
            throw new ArgumentException("Guard does not belong to this society.");

        gate.AssignedGuardId = guard.Id;
        await _db.SaveChangesAsync();

        return $"'{guard.Name}' assigned to gate '{gate.Name}'.";
    }
}
