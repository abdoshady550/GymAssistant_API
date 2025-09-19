namespace GymAssistant_API.Repository.Interfaces.Identity
{
    public interface IEmailService
    {
        Task SendPasswordResetEmailAsync(string email, string resetToken);
    }
}
