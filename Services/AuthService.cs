using Microsoft.EntityFrameworkCore;
using Pravesh.API.Data;
using Pravesh.API.DTOs.Auth;
using Pravesh.API.Entities;
using Pravesh.API.Helpers;
using Pravesh.API.Services.Interfaces;

namespace Pravesh.API.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _db;
    private readonly JwtService _jwt;

    public AuthService(AppDbContext db, JwtService jwt)
    {
        _db  = db;
        _jwt = jwt;
    }

    // ── Register ──────────────────────────────────────────────────────────────
    public async Task<AuthResponse> RegisterAsync(RegisterRequest req)
    {
        // Check duplicate email
        if (await _db.Users.AnyAsync(u => u.Email == req.Email))
            throw new ArgumentException("Email already registered.");

        var user = new User
        {
            Name         = req.Name,
            Email        = req.Email,
            Phone        = req.Phone,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.Password),
            Role         = req.Role,
            SocietyId    = null,   // assigned later by SUPER_ADMIN
            FlatId       = null,   // assigned later by SOCIETY_ADMIN
            IsActive     = true,
            CreatedAt    = DateTime.UtcNow
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        var token = _jwt.GenerateToken(user);

        return new AuthResponse
        {
            Token     = token,
            Name      = user.Name,
            Email     = user.Email,
            Role      = user.Role.ToString(),
            UserId    = user.Id,
            FlatId    = null,
            SocietyId = null
        };
    }

    // ── Login ─────────────────────────────────────────────────────────────────
    public async Task<AuthResponse> LoginAsync(LoginRequest req)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == req.Email);

        if (user == null || !BCrypt.Net.BCrypt.Verify(req.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid email or password.");

        if (!user.IsActive)
            throw new UnauthorizedAccessException("Account is deactivated. Contact admin.");

        var token = _jwt.GenerateToken(user);

        return new AuthResponse
        {
            Token     = token,
            Name      = user.Name,
            Email     = user.Email,
            Role      = user.Role.ToString(),
            UserId    = user.Id,
            FlatId    = user.FlatId,
            SocietyId = user.SocietyId
        };
    }

    // ── GetMe ─────────────────────────────────────────────────────────────────
    public async Task<UserProfileResponse> GetMeAsync(int userId)
    {
        var user = await _db.Users.FindAsync(userId);

        if (user == null)
            throw new KeyNotFoundException("User not found.");

        return new UserProfileResponse
        {
            Id        = user.Id,
            Name      = user.Name,
            Email     = user.Email,
            Phone     = user.Phone,
            Role      = user.Role.ToString(),
            FlatId    = user.FlatId,
            SocietyId = user.SocietyId,
            IsActive  = user.IsActive,
            CreatedAt = user.CreatedAt
        };
    }

    // ── UpdateMe ──────────────────────────────────────────────────────────────
    public async Task UpdateMeAsync(int userId, UpdateProfileRequest req)
    {
        var user = await _db.Users.FindAsync(userId);

        if (user == null)
            throw new KeyNotFoundException("User not found.");

        user.Name  = req.Name;
        user.Phone = req.Phone;

        await _db.SaveChangesAsync();
    }
}
