namespace Pravesh.API.Services.Interfaces;

public interface IEmailService
{
    Task SendPassCreatedEmailAsync(
        string toEmail,
        string residentName,
        string visitorName,
        string flatNumber,
        DateTime validFrom,
        DateTime validUntil,
        string passUuid
    );

    Task SendEntryAlertEmailAsync(
        string toEmail,
        string residentName,
        string visitorName,
        string gateName,
        DateTime entryTime
    );
}