using Pravesh.API.DTOs.Society;

namespace Pravesh.API.Services.Interfaces;

public interface IGateService
{
    Task<GateResponse> AddGateAsync(int societyId, CreateGateRequest req);

    Task<string> AssignGuardAsync(int gateId, AssignGuardRequest req);
}
