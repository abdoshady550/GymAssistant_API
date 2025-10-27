using GymAssistant_API.Model.Entities.Chat;
using GymAssistant_API.Model.Results;
using GymAssistant_API.Req_Res.Response.Chat;

namespace GymAssistant_API.Repository.Interfaces.Chat
{
    public interface IChatService
    {
        Task<Result<ChatConversationDto>> GetOrCreateConversationAsync(
            string currentUserId, string otherUserId, CancellationToken ct = default);

        Task<Result<List<ChatConversationDto>>> GetUserConversationsAsync(
            string userId, int pageSize = 20, int pageNumber = 1, CancellationToken ct = default);

        Task<Result<List<ChatMessageDto>>> GetConversationMessagesAsync(
            string userId, Guid conversationId, int pageSize = 50, int pageNumber = 1, CancellationToken ct = default);

        Task<Result<int>> GetUnreadMessageCountAsync(
            string userId, Guid? conversationId = null, CancellationToken ct = default);

        Task<Result<ChatMessageDto>> SendMessageAsync(
            string userId, Guid conversationId, string content,
            MessageType type = MessageType.Text, IFormFile? attachment = null, CancellationToken ct = default);
    }

}
