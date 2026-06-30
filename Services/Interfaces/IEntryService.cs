using Pravesh.API.DTOs.Entry;

namespace Pravesh.API.Services.Interfaces;

public interface IEntryService
{
    Task<object> ConfirmEntryAsync(int guardId, ConfirmEntryRequest req);

    Task<List<EntryLogResponse>> GetMyGateEntriesAsync(int guardId);

    Task<List<EntryLogResponse>> GetFlatEntriesAsync(int flatId);

    Task<List<EntryLogResponse>> GetAllEntriesAsync(int? societyId);
}
