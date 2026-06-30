using Microsoft.EntityFrameworkCore;
using Pravesh.API.Data;
using Pravesh.API.DTOs.Society;
using Pravesh.API.Entities;
using Pravesh.API.Enums;
using Pravesh.API.Services.Interfaces;

namespace Pravesh.API.Services;

public class FlatService : IFlatService
{
    private readonly AppDbContext _db;

    public FlatService(AppDbContext db)
    {
        _db = db;
    }

    // ── AddFlat ───────────────────────────────────────────────────────────────
    public async Task<FlatResponse> AddFlatAsync(int societyId, CreateFlatRequest req)
    {
        var societyExists = await _db.Societies.AnyAsync(s => s.Id == societyId);
        if (!societyExists)
            throw new KeyNotFoundException("Society not found.");

        var duplicate = await _db.Flats.AnyAsync(
            f => f.SocietyId == societyId && f.FlatNumber == req.FlatNumber);
        if (duplicate)
            throw new ArgumentException("Flat number already exists.");

        var flat = new Flat
        {
            SocietyId  = societyId,
            FlatNumber = req.FlatNumber,
            Tower      = req.Tower,
            Floor      = req.Floor
        };

        _db.Flats.Add(flat);
        await _db.SaveChangesAsync();

        return new FlatResponse
        {
            Id         = flat.Id,
            SocietyId  = flat.SocietyId,
            FlatNumber = flat.FlatNumber,
            Tower      = flat.Tower,
            Floor      = flat.Floor
        };
    }

    // ── AssignResident ────────────────────────────────────────────────────────
    public async Task<string> AssignResidentAsync(int flatId, AssignResidentRequest req)
    {
        var flat = await _db.Flats.FindAsync(flatId);
        if (flat == null)
            throw new KeyNotFoundException("Flat not found.");

        var resident = await _db.Users.FindAsync(req.ResidentId);
        if (resident == null)
            throw new KeyNotFoundException("User not found.");

        if (resident.Role != UserRole.RESIDENT)
            throw new ArgumentException("User is not a RESIDENT.");

        if (resident.SocietyId != flat.SocietyId)
            throw new ArgumentException("Resident does not belong to this society.");

        // Already assigned to this flat
        if (flat.ResidentId == resident.Id)
            return $"'{resident.Name}' is already assigned to flat '{flat.FlatNumber}'.";

        // If the flat already has a resident
        if (flat.ResidentId != null)
        {
            var currentResident = await _db.Users.FindAsync(flat.ResidentId);

            if (currentResident == null)
                throw new InvalidOperationException("Current resident not found.");

            // Don't modify if current resident belongs to another society
            if (currentResident.SocietyId != resident.SocietyId)
            {
                throw new ArgumentException(
                    "This flat is occupied by a resident from another society. Please unassign the resident first.");
            }

            // Same society -> unassign
            currentResident.FlatId = null;
            flat.ResidentId = null;
        }

        // If the new resident is already assigned to another flat
        if (resident.FlatId != null && resident.FlatId != flat.Id)
        {
            var previousFlat = await _db.Flats.FindAsync(resident.FlatId);

            if (previousFlat == null)
                throw new InvalidOperationException("Resident's current flat not found.");

            // Don't modify if previous flat belongs to another society
            if (previousFlat.SocietyId != flat.SocietyId)
            {
                throw new ArgumentException(
                    $"Resident is already assigned to flat '{previousFlat.FlatNumber}' in another society. Please unassign the resident from the previous society first.");
            }

            // Same society -> unassign
            previousFlat.ResidentId = null;
            resident.FlatId = null;
        }

        // Assign resident to the flat
        flat.ResidentId = resident.Id;
        resident.FlatId = flat.Id;

        await _db.SaveChangesAsync();

        return $"Resident '{resident.Name}' assigned to flat '{flat.FlatNumber}' successfully.";
    }
}
