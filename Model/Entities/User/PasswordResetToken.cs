namespace GymAssistant_API.Model.Entities.User
{
    public class PasswordResetToken : Entity
    {

        public string Email { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiryDate { get; set; }
        public bool IsUsed { get; set; } = false;


    }
}
