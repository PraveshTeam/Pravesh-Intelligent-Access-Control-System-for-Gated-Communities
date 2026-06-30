using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pravesh.API.DTOs.Society;
using Pravesh.API.Services.Interfaces;

namespace Pravesh.API.Controllers;

[ApiController]
[Route("api")]
public class FlatController : ControllerBase
{
    private readonly IFlatService _flatService;

    public FlatController(IFlatService flatService)
    {
        _flatService = flatService;
    }

    // POST /api/societies/{id}/flats
    [HttpPost("societies/{id}/flats")]
    [Authorize(Roles = "SOCIETY_ADMIN,SUPER_ADMIN")]
    public async Task<IActionResult> AddFlat(int id, [FromBody] CreateFlatRequest req)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var flat = await _flatService.AddFlatAsync(id, req);
            return Ok(flat);
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

    // PUT /api/flats/{id}/assign
    [HttpPut("flats/{id}/assign")]
    [Authorize(Roles = "SOCIETY_ADMIN,SUPER_ADMIN")]
    public async Task<IActionResult> AssignResident(int id, [FromBody] AssignResidentRequest req)
    {
        try
        {
            var message = await _flatService.AssignResidentAsync(id, req);
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
