using Pravesh.API.DTOs.Auth;

namespace Pravesh.API.Services.Interfaces;

public interface IAdminService
{
    Task<List<UserProfileResponse>> GetAllUsersAsync(int? societyId);

    Task<string> ToggleUserStatusAsync(int id, bool isActive);
}
