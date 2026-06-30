using Microsoft.EntityFrameworkCore;
using Pravesh.API.Data;
using Pravesh.API.DTOs.Society;
using Pravesh.API.Entities;
using Pravesh.API.Enums;
using Pravesh.API.Services.Interfaces;

namespace Pravesh.API.Services;

public class SocietyService : ISocietyService
{
    private readonly AppDbContext _db;

    public SocietyService(AppDbContext db)
    {
        _db = db;
    }

    // ── CreateSociety ─────────────────────────────────────────────────────────
    public async Task<SocietyResponse> CreateSocietyAsync(CreateSocietyRequest req)
    {
        // ── Duplicate check ───────────────────────────────────────────────────────
        var duplicate = await _db.Societies.AnyAsync(s =>
            s.Name.ToLower() == req.Name.ToLower() &&
            s.City.ToLower() == req.City.ToLower());

        if (duplicate)
            throw new ArgumentException($"Society '{req.Name}' already exists in {req.City}.");

        var society = new Society
        {
            Name = req.Name,
            Address = req.Address,
            City = req.City,
            IsActive = true,
            CreatedAt = DateTime.Now
        };

        _db.Societies.Add(society);
        await _db.SaveChangesAsync();

        return new SocietyResponse
        {
            Id = society.Id,
            Name = society.Name,
            Address = society.Address,
            City = society.City,
            IsActive = society.IsActive,
            CreatedAt = society.CreatedAt
        };
    }

    // ── GetFlats ──────────────────────────────────────────────────────────────
    public async Task<List<FlatResponse>> GetFlatsAsync(int id)
    {
        var societyExists = await _db.Societies.AnyAsync(s => s.Id == id);
        if (!societyExists)
            throw new KeyNotFoundException("Society not found.");

        return await _db.Flats
            .Where(f => f.SocietyId == id)
            .Include(f => f.Resident)
            .Select(f => new FlatResponse
            {
                Id = f.Id,
                SocietyId = f.SocietyId,
                FlatNumber = f.FlatNumber,
                Tower = f.Tower,
                Floor = f.Floor,
                ResidentId = f.ResidentId,
                ResidentName = f.Resident != null ? f.Resident.Name : null
            })
            .ToListAsync();
    }

    // ── GetGates ──────────────────────────────────────────────────────────────
    public async Task<List<GateResponse>> GetGatesAsync(int id)
    {
        var societyExists = await _db.Societies.AnyAsync(s => s.Id == id);
        if (!societyExists)
            throw new KeyNotFoundException("Society not found.");

        return await _db.Gates
            .Where(g => g.SocietyId == id)
            .Include(g => g.AssignedGuard)
            .Select(g => new GateResponse
            {
                Id = g.Id,
                SocietyId = g.SocietyId,
                Name = g.Name,
                Location = g.Location,
                AssignedGuardId = g.AssignedGuardId,
                AssignedGuardName = g.AssignedGuard != null ? g.AssignedGuard.Name : null,
                IsActive = g.IsActive
            })
            .ToListAsync();
    }

    // ── AssignSocietyAdmin ────────────────────────────────────────────────────
    public async Task<string> AssignSocietyAdminAsync(int id, AssignSocietyAdminRequest req)
    {
        var society = await _db.Societies.FindAsync(id);
        if (society == null)
            throw new KeyNotFoundException("Society not found.");

        var user = await _db.Users.FindAsync(req.UserId);
        if (user == null)
            throw new KeyNotFoundException("User not found.");

        if (user.Role != UserRole.SOCIETY_ADMIN)
            throw new ArgumentException("User is not a SOCIETY_ADMIN.");

        society.AdminId = user.Id;
        user.SocietyId = society.Id;
        await _db.SaveChangesAsync();

        return $"'{user.Name}' assigned as admin of '{society.Name}'.";
    }

    // ── AssignUserToSociety ───────────────────────────────────────────────────
    public async Task<string> AssignUserToSocietyAsync(int id, AssignUserToSocietyRequest req)
    {
        var society = await _db.Societies.FindAsync(id);
        if (society == null)
            throw new KeyNotFoundException("Society not found.");

        var user = await _db.Users.FindAsync(req.UserId);
        if (user == null)
            throw new KeyNotFoundException("User not found.");

        if (user.Role == UserRole.SUPER_ADMIN)
            throw new ArgumentException("SUPER_ADMIN cannot be assigned to a society.");

        if (user.SocietyId != null)
            throw new ArgumentException("User is already assigned to a society.");

        user.SocietyId = society.Id;
        await _db.SaveChangesAsync();

        return $"'{user.Name}' ({user.Role}) added to society '{society.Name}'.";
    }
}
