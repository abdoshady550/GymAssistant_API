using GymAssistant_API.Repository.Interfaces.Identity;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
//using System.Net;
//using System.Net.Mail;

namespace GymAssistant_API.Repository.Services.Identity
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendPasswordResetEmailAsync(string email, string resetToken)
        {
            try
            {
                var smtpServer = _configuration["EmailSettings:SmtpServer"];
                var smtpPort = int.Parse(_configuration["EmailSettings:Port"] ?? "587");
                var senderEmail = _configuration["EmailSettings:SenderEmail"];
                var senderPassword = _configuration["EmailSettings:SenderPassword"];
                var frontendUrl = _configuration["EmailSettings:FrontendUrl"];

                var resetUrl = $"{frontendUrl}/reset-password?token={resetToken}&email={Uri.EscapeDataString(email)}";

                // Create message
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("FitrixApp Support", "appfitrix@gmail.com"));
                message.To.Add(new MailboxAddress("", email));
                message.Subject = "Password Reset Request - FitrixApp";

                // Create HTML and text body
                var bodyBuilder = new BodyBuilder
                {
                    HtmlBody = $@"
                        <!DOCTYPE html>
                        <html>
                        <head>
                            <meta charset='utf-8'>
                            <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                        </head>
                        <body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333; max-width: 600px; margin: 0 auto; padding: 20px;'>
                            <div style='background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); padding: 30px; border-radius: 10px; text-align: center; margin-bottom: 30px;'>
                                <h1 style='color: white; margin: 0; font-size: 28px;'>🏋️ FitrixApp</h1>
                                <p style='color: #f0f0f0; margin: 10px 0 0 0; font-size: 16px;'>Password Reset Request</p>
                            </div>
                            
                            <div style='background-color: #f9f9f9; padding: 30px; border-radius: 8px; margin-bottom: 20px;'>
                                <h2 style='color: #333; margin-top: 0;'>Reset Your Password</h2>
                                <p style='color: #666; font-size: 16px; margin-bottom: 25px;'>
                                    We received a request to reset your password for your FitrixApp account. 
                                    Click the button below to create a new password:
                                </p>
                                
                                <div style='text-align: center; margin: 35px 0;'>
                                    <a href='{resetUrl}' 
                                       style='background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); 
                                              color: white; 
                                              padding: 15px 35px; 
                                              text-decoration: none; 
                                              display: inline-block; 
                                              border-radius: 25px; 
                                              font-weight: bold; 
                                              font-size: 16px;
                                              box-shadow: 0 4px 15px rgba(102, 126, 234, 0.3);'>
                                        🔑 Reset Password
                                    </a>
                                </div>
                                
                                <div style='background-color: #fff3cd; border-left: 4px solid #ffc107; padding: 15px; margin: 25px 0; border-radius: 4px;'>
                                    <p style='margin: 0; color: #856404; font-size: 14px;'>
                                        ⚠️ <strong>Important:</strong> This link will expire in 1 hour for security reasons.
                                    </p>
                                </div>
                                
                                <p style='color: #666; font-size: 14px; margin-bottom: 0;'>
                                    If the button doesn't work, copy and paste this link into your browser:
                                </p>
                                <p style='background-color: #f1f1f1; padding: 10px; border-radius: 4px; word-break: break-all; font-size: 12px; color: #555;'>
                                    {resetUrl}
                                </p>
                            </div>
                            
                            <div style='background-color: #f8f9fa; padding: 20px; border-radius: 8px; border-left: 4px solid #dc3545;'>
                                <h3 style='color: #721c24; margin-top: 0; font-size: 16px;'>🔒 Security Notice</h3>
                                <p style='color: #721c24; font-size: 14px; margin: 0;'>
                                    If you didn't request this password reset, please ignore this email. 
                                    Your account remains secure and no changes will be made.
                                </p>
                            </div>
                            
                            <hr style='margin: 30px 0; border: none; border-top: 1px solid #ddd;'>
                            
                            <div style='text-align: center; color: #999; font-size: 12px;'>
                                <p>This email was sent by FitrixApp</p>
                                <p>© 2025 FitrixApp. All rights reserved.</p>
                            </div>
                        </body>
                        </html>",

                    TextBody = $@"
FitrixApp - Password Reset Request
================================

Hello,

We received a request to reset your password for your FitrixApp account.

To reset your password, please visit this link:
{resetUrl}

IMPORTANT: This link will expire in 1 hour for security reasons.

If you didn't request this password reset, please ignore this email. Your account remains secure.

---
FitrixApp Team
© 2025 FitrixApp. All rights reserved.
"
                };

                message.Body = bodyBuilder.ToMessageBody();

                // Send email using MailKit
                using var client = new SmtpClient();

                try
                {
                    // Connect to Outlook SMTP server
                    _logger.LogInformation($"Connecting to SMTP server: {smtpServer}:{smtpPort}");
                    await client.ConnectAsync(smtpServer, smtpPort, SecureSocketOptions.StartTls);

                    // Authenticate
                    _logger.LogInformation($"Authenticating with email: {senderEmail}");
                    await client.AuthenticateAsync(senderEmail, senderPassword);

                    // Send the email
                    _logger.LogInformation($"Sending email to: {email}");
                    await client.SendAsync(message);

                    _logger.LogInformation($"Password reset email sent successfully to: {email}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"SMTP operation failed for email: {email}");
                    throw;
                }
                finally
                {
                    if (client.IsConnected)
                    {
                        await client.DisconnectAsync(true);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send password reset email to: {email}");
                throw new InvalidOperationException($"Failed to send email to {email}. Please check email configuration.", ex);
            }
        }
    }
}
