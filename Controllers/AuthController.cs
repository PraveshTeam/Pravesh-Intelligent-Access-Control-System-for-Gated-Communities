using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pravesh.API.Data;
using Pravesh.API.DTOs.Auth;
using Pravesh.API.Entities;
using Pravesh.API.Helpers;
using System.Security.Claims;

namespace Pravesh.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly JwtService _jwt;

    public AuthController(AppDbContext db, JwtService jwt)
    {
        _db = db;
        _jwt = jwt;
    }

    // ── POST /api/auth/register ───────────────────────────────────────────────
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest req)
    {
        if (req == null)
            return BadRequest(new { success = false, message = "Request body is required." });

        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        // Check duplicate email
        if (await _db.Users.AnyAsync(u => u.Email == req.Email))
            return BadRequest(new { success = false, message = "Email already registered." });

        // Validate society exists if provided
        if (req.SocietyId.HasValue)
        {
            var societyExists = await _db.Societies.AnyAsync(s => s.Id == req.SocietyId.Value);
            if (!societyExists)
                return BadRequest(new { success = false, message = "Society not found." });
        }

        // Validate flat exists if provided
        if (req.FlatId.HasValue)
        {
            var flatExists = await _db.Flats.AnyAsync(f => f.Id == req.FlatId.Value);
            if (!flatExists)
                return BadRequest(new { success = false, message = "Flat not found." });
        }

        var user = new User
        {
            Name         = req.Name,
            Email        = req.Email,
            Phone        = req.Phone,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.Password),
            Role         = req.Role,
            SocietyId    = req.SocietyId,
            FlatId       = req.FlatId,
            IsActive     = true,
            CreatedAt    = DateTime.UtcNow
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        var token = _jwt.GenerateToken(user);

        return Ok(new AuthResponse
        {
            Token     = token,
            Name      = user.Name,
            Email     = user.Email,
            Role      = user.Role.ToString(),
            UserId    = user.Id,
            FlatId    = user.FlatId,
            SocietyId = user.SocietyId
        });
    }

    // ── POST /api/auth/login ──────────────────────────────────────────────────
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest req)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == req.Email);

        if (user == null || !BCrypt.Net.BCrypt.Verify(req.Password, user.PasswordHash))
            return Unauthorized(new { success = false, message = "Invalid email or password." });

        if (!user.IsActive)
            return Unauthorized(new { success = false, message = "Account is deactivated. Contact admin." });

        var token = _jwt.GenerateToken(user);

        return Ok(new AuthResponse
        {
            Token     = token,
            Name      = user.Name,
            Email     = user.Email,
            Role      = user.Role.ToString(),
            UserId    = user.Id,
            FlatId    = user.FlatId,
            SocietyId = user.SocietyId
        });
    }

    // ── GET /api/users/me ─────────────────────────────────────────────────────
    [Authorize]
    [HttpGet("/api/users/me")]
    public async Task<IActionResult> GetMe()
    {
        var userId = int.Parse(User.FindFirstValue("userId")!);
        var user   = await _db.Users.FindAsync(userId);

        if (user == null)
            return NotFound(new { success = false, message = "User not found." });

        return Ok(new UserProfileResponse
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
        });
    }

    // ── PUT /api/users/me ─────────────────────────────────────────────────────
    [Authorize]
    [HttpPut("/api/users/me")]
    public async Task<IActionResult> UpdateMe([FromBody] UpdateProfileRequest req)
    {
        var userId = int.Parse(User.FindFirstValue("userId")!);
        var user   = await _db.Users.FindAsync(userId);

        if (user == null)
            return NotFound(new { success = false, message = "User not found." });

        user.Name  = req.Name;
        user.Phone = req.Phone;

        await _db.SaveChangesAsync();

        return Ok(new { success = true, message = "Profile updated successfully." });
    }
}