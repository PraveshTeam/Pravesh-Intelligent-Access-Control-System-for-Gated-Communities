using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pravesh.API.DTOs.Society;
using Pravesh.API.Services.Interfaces;

namespace Pravesh.API.Controllers;

[ApiController]
[Route("api")]
public class GateController : ControllerBase
{
    private readonly IGateService _gateService;

    public GateController(IGateService gateService)
    {
        _gateService = gateService;
    }

    // POST /api/societies/{id}/gates
    [HttpPost("societies/{id}/gates")]
    [Authorize(Roles = "SOCIETY_ADMIN,SUPER_ADMIN")]
    public async Task<IActionResult> AddGate(int id, [FromBody] CreateGateRequest req)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var gate = await _gateService.AddGateAsync(id, req);
            return Ok(gate);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { success = false, message = ex.Message });
        }
    }

    // PUT /api/gates/{id}/assign
    [HttpPut("gates/{id}/assign")]
    [Authorize(Roles = "SOCIETY_ADMIN,SUPER_ADMIN")]
    public async Task<IActionResult> AssignGuard(int id, [FromBody] AssignGuardRequest req)
    {
        try
        {
            var message = await _gateService.AssignGuardAsync(id, req);
            return Ok(new { success = true, message });
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
