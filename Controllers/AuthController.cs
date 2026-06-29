using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pravesh.API.DTOs.Auth;
using Pravesh.API.Services.Interfaces;
using System.Security.Claims;

namespace Pravesh.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    private int GetUserId() =>
        int.Parse(User.FindFirstValue("userId")!);

    // ── POST /api/auth/register ───────────────────────────────────────────────
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest req)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var result = await _authService.RegisterAsync(req);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    // ── POST /api/auth/login ──────────────────────────────────────────────────
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest req)
    {
        try
        {
            var result = await _authService.LoginAsync(req);
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { success = false, message = ex.Message });
        }
    }

    // ── GET /api/users/me ─────────────────────────────────────────────────────
    [Authorize]
    [HttpGet("/api/users/me")]
    public async Task<IActionResult> GetMe()
    {
        try
        {
            var result = await _authService.GetMeAsync(GetUserId());
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { success = false, message = ex.Message });
        }
    }

    // ── PUT /api/users/me ─────────────────────────────────────────────────────
    [Authorize]
    [HttpPut("/api/users/me")]
    public async Task<IActionResult> UpdateMe([FromBody] UpdateProfileRequest req)
    {
        try
        {
            await _authService.UpdateMeAsync(GetUserId(), req);
            return Ok(new { success = true, message = "Profile updated successfully." });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { success = false, message = ex.Message });
        }
    }
}
