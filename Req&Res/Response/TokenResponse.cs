using GymAssistant_API.Req_Res.Response;

namespace GymAssistant_API.Model.Identity.Dtos
{
    public class TokenResponse
    {
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime ExpiresOnUtc { get; set; }
        public AppUserDto? User { get; set; } = null;
    }
}
