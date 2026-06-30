using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pravesh.API.DTOs.Society;
using Pravesh.API.Services.Interfaces;

namespace Pravesh.API.Controllers;

[ApiController]
[Route("api/societies")]
public class SocietyController : ControllerBase
{
    private readonly ISocietyService _societyService;

    public SocietyController(ISocietyService societyService)
    {
        _societyService = societyService;
    }

    // POST /api/societies
    [HttpPost]
    [Authorize(Roles = "SUPER_ADMIN")]
    public async Task<IActionResult> CreateSociety([FromBody] CreateSocietyRequest req)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var result = await _societyService.CreateSocietyAsync(req);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    // GET /api/societies/{id}/flats
    [HttpGet("{id}/flats")]
    [Authorize(Roles = "SOCIETY_ADMIN,SUPER_ADMIN")]
    public async Task<IActionResult> GetFlats(int id)
    {
        try
        {
            var flats = await _societyService.GetFlatsAsync(id);
            return Ok(flats);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { success = false, message = ex.Message });
        }
    }

    // GET /api/societies/{id}/gates
    [HttpGet("{id}/gates")]
    [Authorize(Roles = "SOCIETY_ADMIN,SUPER_ADMIN")]
    public async Task<IActionResult> GetGates(int id)
    {
        try
        {
            var gates = await _societyService.GetGatesAsync(id);
            return Ok(gates);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { success = false, message = ex.Message });
        }
    }

    // PUT /api/societies/{id}/assign-admin
    [HttpPut("{id}/assign-admin")]
    [Authorize(Roles = "SUPER_ADMIN")]
    public async Task<IActionResult> AssignSocietyAdmin(int id, [FromBody] AssignSocietyAdminRequest req)
    {
        try
        {
            var message = await _societyService.AssignSocietyAdminAsync(id, req);
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

    // PUT /api/societies/{id}/assign-user
    [HttpPut("{id}/assign-user")]
    [Authorize(Roles = "SUPER_ADMIN,SOCIETY_ADMIN")]
    public async Task<IActionResult> AssignUserToSociety(int id, [FromBody] AssignUserToSocietyRequest req)
    {
        try
        {
            var message = await _societyService.AssignUserToSocietyAsync(id, req);
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
