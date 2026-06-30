using Pravesh.API.DTOs.Society;

namespace Pravesh.API.Services.Interfaces;

public interface ISocietyService
{
    Task<SocietyResponse> CreateSocietyAsync(CreateSocietyRequest req);

    Task<List<FlatResponse>> GetFlatsAsync(int id);

    Task<List<GateResponse>> GetGatesAsync(int id);

    Task<string> AssignSocietyAdminAsync(int id, AssignSocietyAdminRequest req);

    Task<string> AssignUserToSocietyAsync(int id, AssignUserToSocietyRequest req);
}
