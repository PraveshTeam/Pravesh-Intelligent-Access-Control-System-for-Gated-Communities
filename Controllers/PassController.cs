using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pravesh.API.DTOs.Pass;
using Pravesh.API.Services.Interfaces;
using System.Security.Claims;

namespace Pravesh.API.Controllers;

[ApiController]
[Route("api/passes")]
[Authorize(Roles = "RESIDENT")]
public class PassController : ControllerBase
{
    private readonly IPassService _passService;

    public PassController(IPassService passService)
    {
        _passService = passService;
    }

    private int GetUserId() =>
        int.Parse(User.FindFirstValue("userId")!);

    // ── POST /api/passes ──────────────────────────────────────────────────────
    [HttpPost]
    public async Task<IActionResult> CreatePass([FromBody] CreatePassRequest req)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var pass = await _passService.CreatePassAsync(GetUserId(), req);
            return Ok(pass);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { success = false, message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    // ── GET /api/passes ───────────────────────────────────────────────────────
    [HttpGet]
    public async Task<IActionResult> GetMyActivePasses()
    {
        var passes = await _passService.GetMyActivePassesAsync(GetUserId());
        return Ok(passes);
    }

    // ── GET /api/passes/history ───────────────────────────────────────────────
    [HttpGet("history")]
    public async Task<IActionResult> GetMyPassHistory()
    {
        var passes = await _passService.GetMyPassHistoryAsync(GetUserId());
        return Ok(passes);
    }

    // ── GET /api/passes/{id} ──────────────────────────────────────────────────
    [HttpGet("{id}")]
    public async Task<IActionResult> GetPassById(int id)
    {
        try
        {
            var pass = await _passService.GetPassByIdAsync(GetUserId(), id);
            return Ok(pass);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { success = false, message = ex.Message });
        }
    }

    // ── DELETE /api/passes/{id} ───────────────────────────────────────────────
    [HttpDelete("{id}")]
    public async Task<IActionResult> RevokePass(int id)
    {
        try
        {
            await _passService.RevokePassAsync(GetUserId(), id);
            return Ok(new { success = true, message = "Pass revoked successfully." });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { success = false, message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }
}
