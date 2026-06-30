using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pravesh.API.DTOs.Entry;
using Pravesh.API.Services.Interfaces;
using System.Security.Claims;

namespace Pravesh.API.Controllers;

[ApiController]
[Route("api/entries")]
public class EntryController : ControllerBase
{
    private readonly IEntryService _entryService;

    public EntryController(IEntryService entryService)
    {
        _entryService = entryService;
    }

    private int GetUserId() =>
        int.Parse(User.FindFirstValue("userId")!);

    // ── POST /api/entries/confirm ─────────────────────────────────────────────
    [HttpPost("confirm")]
    [Authorize(Roles = "GUARD")]
    public async Task<IActionResult> ConfirmEntry([FromBody] ConfirmEntryRequest req)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _entryService.ConfirmEntryAsync(GetUserId(), req);
        return Ok(result);
    }

    // ── GET /api/entries ──────────────────────────────────────────────────────
    [HttpGet]
    [Authorize(Roles = "GUARD")]
    public async Task<IActionResult> GetMyGateEntries()
    {
        var logs = await _entryService.GetMyGateEntriesAsync(GetUserId());
        return Ok(logs);
    }

    // ── GET /api/entries/flat/{flatId} ────────────────────────────────────────
    [HttpGet("flat/{flatId}")]
    [Authorize(Roles = "RESIDENT")]
    public async Task<IActionResult> GetFlatEntries(int flatId)
    {
        var logs = await _entryService.GetFlatEntriesAsync(flatId);
        return Ok(logs);
    }

    // ── GET /api/admin/entries ────────────────────────────────────────────────
    [HttpGet("/api/admin/entries")]
    [Authorize(Roles = "SOCIETY_ADMIN,SUPER_ADMIN")]
    public async Task<IActionResult> GetAllEntries([FromQuery] int? societyId)
    {
        var logs = await _entryService.GetAllEntriesAsync(societyId);
        return Ok(logs);
    }
}
