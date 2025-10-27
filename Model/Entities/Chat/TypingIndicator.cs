namespace GymAssistant_API.Model.Entities.Chat
{
    public class TypingIndicator
    {
        public Guid ConversationId { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public DateTime StartedAt { get; set; } = DateTime.UtcNow;
    }

}
