namespace GymAssistant_API.Req_Res.Reqeust.User
{
    public record ChangePasswordRequest(
        string CurrentPassword,
        string NewPassword,
        string ConfirmPassword
        );
    public record ChangeSeedingPasswordRequest(
    string NewPassword
    );
}
