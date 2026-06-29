using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Pravesh.API.Services.Interfaces;

namespace Pravesh.API.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;

    public EmailService(IConfiguration config)
    {
        _config = config;
    }

    private async Task SendEmailAsync(string toEmail, string toName, string subject, string htmlBody)
    {
        var message = new MimeMessage();

        message.From.Add(new MailboxAddress(
            _config["Email:SenderName"],
            _config["Email:SenderEmail"]));

        message.To.Add(new MailboxAddress(toName, toEmail));
        message.Subject = subject;
        message.Body    = new TextPart("html") { Text = htmlBody };

        using var smtp = new SmtpClient();

        await smtp.ConnectAsync(
            _config["Email:SmtpHost"],
            int.Parse(_config["Email:SmtpPort"]!),
            SecureSocketOptions.StartTls);

        await smtp.AuthenticateAsync(
            _config["Email:SenderEmail"],
            _config["Email:AppPassword"]);

        await smtp.SendAsync(message);
        await smtp.DisconnectAsync(true);
    }

    public async Task SendPassCreatedEmailAsync(
        string toEmail,
        string residentName,
        string visitorName,
        string flatNumber,
        DateTime validFrom,
        DateTime validUntil,
        string passUuid)
    {
        var subject = $"Pravesh — Visitor Pass Created for {visitorName}";

        var body = $"""
            <h2>Visitor Pass Created</h2>
            <p>Dear <b>{residentName}</b>,</p>
            <p>A visitor pass has been created for your flat <b>{flatNumber}</b>.</p>
            <table border='1' cellpadding='8' cellspacing='0'>
                <tr><td><b>Visitor Name</b></td><td>{visitorName}</td></tr>
                <tr><td><b>Pass UUID</b></td><td>{passUuid}</td></tr>
                <tr><td><b>Valid From</b></td><td>{validFrom:dd MMM yyyy hh:mm tt} IST</td></tr>
                <tr><td><b>Valid Until</b></td><td>{validUntil:dd MMM yyyy hh:mm tt} IST</td></tr>
            </table>
            <p>Share the Pass UUID with your visitor for entry.</p>
            <br/>
            <p>— Pravesh Security System</p>
            """;

        await SendEmailAsync(toEmail, residentName, subject, body);
    }

    public async Task SendEntryAlertEmailAsync(
        string toEmail,
        string residentName,
        string visitorName,
        string gateName,
        DateTime entryTime)
    {
        var subject = $"Pravesh — Visitor Entry Alert: {visitorName}";

        var body = $"""
            <h2>Visitor Entry Alert</h2>
            <p>Dear <b>{residentName}</b>,</p>
            <p>Your visitor <b>{visitorName}</b> has entered the society.</p>
            <table border='1' cellpadding='8' cellspacing='0'>
                <tr><td><b>Visitor Name</b></td><td>{visitorName}</td></tr>
                <tr><td><b>Gate</b></td><td>{gateName}</td></tr>
                <tr><td><b>Entry Time</b></td><td>{entryTime:dd MMM yyyy hh:mm tt} IST</td></tr>
            </table>
            <br/>
            <p>— Pravesh Security System</p>
            """;

        await SendEmailAsync(toEmail, residentName, subject, body);
    }
}