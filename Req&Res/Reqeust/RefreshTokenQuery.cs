using System.ComponentModel.DataAnnotations;
namespace GymAssistant_API.Req_Res.Reqeust
{
    public record RefreshTokenQuery(
        [Required(ErrorMessage = "Refresh token is required")]
        [MinLength(10, ErrorMessage = "Refresh token must be at least 10 characters")]
        string RefreshToken,

        [Required(ErrorMessage = "Expired access token is required")]
        string ExpiredAccessToken
    );
}
