using Microsoft.EntityFrameworkCore;
using Pravesh.API.Data;
using Pravesh.API.DTOs.Auth;
using Pravesh.API.Services.Interfaces;

namespace Pravesh.API.Services;

public class AdminService : IAdminService
{
    private readonly AppDbContext _db;

    public AdminService(AppDbContext db)
    {
        _db = db;
    }

    // ── GetAllUsers ───────────────────────────────────────────────────────────
    public async Task<List<UserProfileResponse>> GetAllUsersAsync(int? societyId)
    {
        var query = _db.Users.AsQueryable();

        if (societyId.HasValue)
            query = query.Where(u => u.SocietyId == societyId);

        return await query
            .Select(u => new UserProfileResponse
            {
                Id = u.Id,
                Name = u.Name,
                Email = u.Email,
                Phone = u.Phone,
                Role = u.Role.ToString(),
                FlatId = u.FlatId,
                SocietyId = u.SocietyId,
                IsActive = u.IsActive,
                CreatedAt = u.CreatedAt
            })
            .ToListAsync();
    }

    // ── ToggleUserStatus ──────────────────────────────────────────────────────
    public async Task<string> ToggleUserStatusAsync(int id, bool isActive)
    {
        var user = await _db.Users.FindAsync(id);
        if (user == null)
            throw new KeyNotFoundException("User not found.");

        user.IsActive = isActive;
        await _db.SaveChangesAsync();

        var status = isActive ? "activated" : "deactivated";
        return $"User '{user.Name}' {status}.";
    }
}
