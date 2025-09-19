namespace GymAssistant_API.Model.Identity
{
    public class EmailConfiguration
    {

        public string SmtpServer { get; set; } = null!;
        public int Port { get; set; }
        public string SenderEmail { get; set; } = null!;
        public string SenderPassword { get; set; } = null!;
        public string FrontendUrl { get; set; } = null!;

    }
}
