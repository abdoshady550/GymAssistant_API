using GymAssistant_API.Hubs;
using GymAssistant_API.Model.Entities.Chat;
using GymAssistant_API.Repository.Interfaces.Chat;
using GymAssistant_API.Req_Res.Response.Chat;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace GymAssistant_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatV2Controller : ApiController
    {
        private readonly IChatService _chatService;
        private readonly ILogger<ChatV2Controller> _logger;
        private readonly IHubContext<ChatHub> _hubContext; 

        public ChatV2Controller(
            IChatService chatService,
            ILogger<ChatV2Controller> logger,
            IHubContext<ChatHub> hubContext) 
        {
            _chatService = chatService;
            _logger = logger;
            _hubContext = hubContext; 
        }

        [HttpGet("conversations")]
        [ProducesResponseType(typeof(List<ChatConversationDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [EndpointSummary("Get user's chat conversations")]
        [EndpointDescription("Retrieves all chat conversations for the authenticated user with pagination")]
        [EndpointName("GetConversations")]
        public async Task<ActionResult> GetConversations(
            [FromQuery] int pageSize = 20,
            [FromQuery] int pageNumber = 1,
            CancellationToken ct = default)
        {
            var result = await _chatService.GetUserConversationsAsync(
                GetCurrentUserId(), pageSize, pageNumber, ct);

            return result.Match(
                response => Ok(response),
                Problem);
        }

        [HttpGet("conversations/{otherUserId}")]
        [ProducesResponseType(typeof(ChatConversationDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [EndpointSummary("Get or create conversation with specific user")]
        [EndpointDescription("Gets existing conversation or creates new one with trainer/trainee")]
        [EndpointName("GetOrCreateConversation")]
        public async Task<ActionResult> GetOrCreateConversation(
            [FromRoute] string otherUserId,
            CancellationToken ct = default)
        {
            var result = await _chatService.GetOrCreateConversationAsync(
                GetCurrentUserId(), otherUserId, ct);

            return result.Match(
                response => Ok(response),
                Problem);
        }

        [HttpGet("conversations/{conversationId:guid}/messages")]
        [ProducesResponseType(typeof(List<ChatMessageDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [EndpointSummary("Get conversation messages")]
        [EndpointDescription("Retrieves messages from a specific conversation with pagination")]
        [EndpointName("GetConversationMessages")]
        public async Task<ActionResult> GetConversationMessages(
            [FromRoute] Guid conversationId,
            [FromQuery] int pageSize = 50,
            [FromQuery] int pageNumber = 1,
            CancellationToken ct = default)
        {
            var result = await _chatService.GetConversationMessagesAsync(
                GetCurrentUserId(), conversationId, pageSize, pageNumber, ct);

            return result.Match(
                response => Ok(response),
                Problem);
        }

        [HttpGet("unread-count")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [EndpointSummary("Get unread messages count")]
        [EndpointDescription("Returns total unread messages count for the user")]
        [EndpointName("GetUnreadCount")]
        public async Task<ActionResult> GetUnreadCount(
            [FromQuery] Guid? conversationId = null,
            CancellationToken ct = default)
        {
            var result = await _chatService.GetUnreadMessageCountAsync(
                GetCurrentUserId(), conversationId, ct);

            return result.Match(
                response => Ok(response),
                Problem);
        }

        // ✅ الجزء المهم - تحديث SendMessage
        [HttpPost("conversations/{conversationId:guid}/messages")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(ChatMessageDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [EndpointSummary("Send a message")]
        [EndpointDescription("Sends a new message in a conversation (use SignalR for real-time)")]
        [EndpointName("SendMessage")]
        public async Task<ActionResult> SendMessage(
            [FromRoute] Guid conversationId,
            [FromForm] string content,
            [FromForm] string type = "Text",
            [FromForm] IFormFile? attachment = null,
            CancellationToken ct = default)
        {
            // Parse the message type
            if (!Enum.TryParse<MessageType>(type, true, out var messageType))
            {
                messageType = MessageType.Text;
            }

            // 1️⃣ حفظ الرسالة في الـ Database
            var result = await _chatService.SendMessageAsync(
                GetCurrentUserId(), conversationId, content, messageType, attachment, ct);

            if (result.IsError)
            {
                return result.Match(
                    response => Ok(response),
                    Problem);
            }

            var messageDto = result.Value;

            try
            {
                // 2️⃣ إرسال الرسالة عبر SignalR للـ real-time users
                await _hubContext.Clients
                    .Group($"conversation_{conversationId}")
                    .SendAsync("ReceiveMessage", new
                    {
                        messageDto.Id,
                        messageDto.ConversationId,
                        messageDto.SenderId,
                        messageDto.SenderName,
                        messageDto.Content,
                        messageDto.Type,
                        messageDto.AttachmentUrl,
                        messageDto.SentAt,
                        messageDto.IsRead,
                        messageDto.IsEdited
                    }, ct);

                _logger.LogInformation(
                    "Message sent via SignalR to conversation {ConversationId}",
                    conversationId);
            }
            catch (Exception ex)
            {
                // لو SignalR فشل، الرسالة محفوظة في DB على الأقل
                _logger.LogWarning(ex,
                    "Failed to send message via SignalR, but saved in database");
            }

            // 3️⃣ رجوع Response للـ sender
            return Ok(messageDto);
        }

        private string GetCurrentUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier)!;
    }

}

