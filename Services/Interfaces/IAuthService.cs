using Pravesh.API.DTOs.Auth;

namespace Pravesh.API.Services.Interfaces;

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest req);

    Task<AuthResponse> LoginAsync(LoginRequest req);

    Task<UserProfileResponse> GetMeAsync(int userId);

    Task UpdateMeAsync(int userId, UpdateProfileRequest req);
}
