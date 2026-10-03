using GymAssistant_API.Repository.Interfaces.Identity;

namespace GymAssistant.TestCommon.Fakes;

public class FakeEmailService : IEmailService
{
    public List<EmailMessageRecord> SentMessages { get; } = new();

    public Task SendPasswordResetEmailAsync(string email, string resetToken)
    {
        SentMessages.Add(new EmailMessageRecord("Password Reset", resetToken, email));
        return Task.CompletedTask;
    }
}

public record EmailMessageRecord(string Subject, string Body, string Recipient);
