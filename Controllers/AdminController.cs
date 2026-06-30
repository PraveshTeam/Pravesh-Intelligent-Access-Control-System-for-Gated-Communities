using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pravesh.API.Services.Interfaces;

namespace Pravesh.API.Controllers;

[ApiController]
[Route("api/admin")]
[Authorize(Roles = "SOCIETY_ADMIN,SUPER_ADMIN")]
public class AdminController : ControllerBase
{
    private readonly IAdminService _adminService;

    public AdminController(IAdminService adminService)
    {
        _adminService = adminService;
    }

    // GET /api/admin/users
    [HttpGet("users")]
    public async Task<IActionResult> GetAllUsers([FromQuery] int? societyId)
    {
        var users = await _adminService.GetAllUsersAsync(societyId);
        return Ok(users);
    }

    // PUT /api/admin/users/{id}/status
    [HttpPut("users/{id}/status")]
    public async Task<IActionResult> ToggleUserStatus(int id, [FromQuery] bool isActive)
    {
        try
        {
            var message = await _adminService.ToggleUserStatusAsync(id, isActive);
            return Ok(new { success = true, message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { success = false, message = ex.Message });
        }
    }
}
