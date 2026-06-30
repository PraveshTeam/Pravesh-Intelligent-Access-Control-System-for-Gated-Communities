using Pravesh.API.DTOs.Society;

namespace Pravesh.API.Services.Interfaces;

public interface IFlatService
{
    Task<FlatResponse> AddFlatAsync(int societyId, CreateFlatRequest req);

    Task<string> AssignResidentAsync(int flatId, AssignResidentRequest req);
}
