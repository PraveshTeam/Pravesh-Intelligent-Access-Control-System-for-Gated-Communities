using Pravesh.API.DTOs.Pass;

namespace Pravesh.API.Services.Interfaces;

public interface IPassService
{
    Task<PassResponse> CreatePassAsync(int userId, CreatePassRequest req);

    Task<List<PassResponse>> GetMyActivePassesAsync(int userId);

    Task<List<PassResponse>> GetMyPassHistoryAsync(int userId);

    Task<PassResponse> GetPassByIdAsync(int userId, int passId);

    Task RevokePassAsync(int userId, int passId);
}
