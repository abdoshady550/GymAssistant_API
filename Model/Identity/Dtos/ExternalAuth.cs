namespace GymAssistant_API.Model.Identity.Dtos
{

    public class ExternalLoginDto
    {
        public string Provider { get; set; } = null!; // "Google" or "Facebook"
        public string IdToken { get; set; } = null!;
        public string? AccessToken { get; set; }
    }

    public class ExternalLoginCallbackDto
    {
        public string Provider { get; set; } = null!;
        public string ReturnUrl { get; set; } = "/";
    }

    public class ExternalAuthInfoDto
    {
        public string Email { get; set; } = null!;
        public string? Name { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Picture { get; set; }
        public string Provider { get; set; } = null!;
        public string ProviderId { get; set; } = null!;
    }

}
