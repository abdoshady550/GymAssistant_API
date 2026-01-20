using GymAssistant_API.Model.Entities.Chat;

namespace GymAssistant_API.Req_Res.Response.Chat
{
    public class ChatConversationDto
    {
        public Guid Id { get; set; }
        public string OtherUserId { get; set; }
        public string OtherUserName { get; set; } = string.Empty;
        public string? OtherUserImage { get; set; }
        public string? LastMessage { get; set; }
        public DateTime? LastMessageAt { get; set; }
        public int UnreadCount { get; set; }
        public bool IsOnline { get; set; }
        public DateTime CreatedAt { get; set; }

        public static ChatConversationDto FromEntity(
            ChatConversation conversation, Guid currentUserId, int unreadCount)
        {
            var isCurrentUserTrainer = conversation.TrainerId == currentUserId;
            var otherUser = isCurrentUserTrainer ? conversation.Trainee : conversation.Trainer;

            return new ChatConversationDto
            {
                Id = conversation.Id,
                OtherUserId = otherUser.AppUserId,
                OtherUserName = otherUser.FullName,
                OtherUserImage = otherUser.Image,
                LastMessage = conversation.LastMessageText,
                LastMessageAt = conversation.LastMessageAt,
                UnreadCount = unreadCount,
                IsOnline = false, // Will be set by SignalR
                CreatedAt = conversation.CreatedAtUtc.DateTime
            };
        }
    }

    public class ChatMessageDto
    {
        public Guid Id { get; set; }
        public Guid ConversationId { get; set; }
        public string SenderId { get; set; }
        public string SenderName { get; set; } = string.Empty;
        public string? SenderImage { get; set; }
        public string Content { get; set; } = string.Empty;
        public MessageType Type { get; set; }
        public string? AttachmentUrl { get; set; }
        public bool IsRead { get; set; }
        public DateTime? ReadAt { get; set; }
        public DateTime SentAt { get; set; }
        public bool IsEdited { get; set; }
        public DateTime? EditedAt { get; set; }

        public static ChatMessageDto FromEntity(ChatMessage message)
        {
            return new ChatMessageDto
            {
                Id = message.Id,
                ConversationId = message.ConversationId,
                SenderId = message.Sender.AppUserId,
                SenderName = message.Sender?.FullName ?? "Unknown",
                SenderImage = message.Sender?.Image,
                Content = message.Content,
                Type = message.Type,
                AttachmentUrl = message.AttachmentUrl,
                IsRead = message.IsRead,
                ReadAt = message.ReadAt,
                SentAt = message.SentAt,
                IsEdited = message.IsEdited,
                EditedAt = message.EditedAt
            };
        }
    }
}
