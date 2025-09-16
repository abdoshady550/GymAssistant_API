namespace GymAssistant_API.Req_Res.Reqeust
{
    public record RefreshTokenQuery(string RefreshToken, string ExpiredAccessToken);
}
