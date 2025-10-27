using GymAssistant_API.Model.Entities.User;
using GymAssistant_API.Model.Results;

namespace GymAssistant_API.Model.Entities.Chat
{
    public sealed class ChatMessage : Entity
    {
        public Guid ConversationId { get; private set; }
        public ChatConversation Conversation { get; private set; } = default!;

        public Guid SenderId { get; private set; }
        public ClientProfile Sender { get; private set; } = default!;

        public string Content { get; private set; }
        public MessageType Type { get; private set; }
        public string? AttachmentUrl { get; private set; }

        public bool IsRead { get; private set; } = false;
        public DateTime? ReadAt { get; private set; }
        public DateTime SentAt { get; private set; }

        public bool IsEdited { get; private set; } = false;
        public DateTime? EditedAt { get; private set; }

        private ChatMessage() { }

        private ChatMessage(Guid id, Guid conversationId, Guid senderId,
                          string content, MessageType type = MessageType.Text,
                          string? attachmentUrl = null) : base(id)
        {
            ConversationId = conversationId;
            SenderId = senderId;
            Content = content;
            Type = type;
            AttachmentUrl = attachmentUrl;
            SentAt = DateTime.UtcNow;
            CreatedAtUtc = DateTimeOffset.UtcNow;
        }

        public static Result<ChatMessage> Create(Guid id, Guid conversationId,
                                                Guid senderId, string content,
                                                MessageType type = MessageType.Text,
                                                string? attachmentUrl = null)
        {
            if (conversationId == Guid.Empty)
                return Error.Validation("ConversationId_Required", "Conversation ID is required");

            if (senderId == Guid.Empty)
                return Error.Validation("SenderId_Required", "Sender ID is required");

            if (string.IsNullOrWhiteSpace(content) && type == MessageType.Text)
                return Error.Validation("Content_Required", "Message content is required");

            if (type != MessageType.Text && string.IsNullOrWhiteSpace(attachmentUrl))
                return Error.Validation("Attachment_Required", "Attachment URL is required for non-text messages");

            return new ChatMessage(id, conversationId, senderId, content, type, attachmentUrl);
        }

        public void MarkAsRead()
        {
            IsRead = true;
            ReadAt = DateTime.UtcNow;
        }

        public Result<Updated> Edit(string newContent)
        {
            if (string.IsNullOrWhiteSpace(newContent))
                return Error.Validation("Content_Required", "New content is required");

            if (Type != MessageType.Text)
                return Error.Validation("Cannot_Edit", "Can only edit text messages");

            Content = newContent;
            IsEdited = true;
            EditedAt = DateTime.UtcNow;

            return Result.Updated;
        }
    }
    public enum MessageType
    {
        Text = 1,
        Image = 2,
        Document = 3,
        Voice = 4,
        Video = 5
    }

}
