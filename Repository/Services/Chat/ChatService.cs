using GymAssistant_API.Data;
using GymAssistant_API.Model.Entities.Chat;
using GymAssistant_API.Model.Entities.User;
using GymAssistant_API.Model.Results;
using GymAssistant_API.Repository.Interfaces.Chat;
using GymAssistant_API.Req_Res.Response.Chat;
using Microsoft.EntityFrameworkCore;

namespace GymAssistant_API.Repository.Services.Chat
{
    public class ChatService : IChatService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ChatService> _logger;
        private readonly IWebHostEnvironment _environment;

        public ChatService(AppDbContext context, ILogger<ChatService> logger, IWebHostEnvironment environment)
        {
            _context = context;
            _logger = logger;
            _environment = environment;
        }

        public async Task<Result<ChatConversationDto>> GetOrCreateConversationAsync(
            string currentUserId, string otherUserId, CancellationToken ct = default)
        {
            try
            {
                var currentProfile = await _context.ClientProfiles
                    .FirstOrDefaultAsync(p => p.AppUserId == currentUserId, ct);

                var otherProfile = await _context.ClientProfiles
                    .FirstOrDefaultAsync(p => p.AppUserId == otherUserId, ct);

                if (currentProfile == null || otherProfile == null)
                    return Error.NotFound("Profile_NotFound", "User profile not found");

                // Check if conversation exists
                var conversation = await _context.ChatConversations
                    .Include(c => c.Trainer)
                    .Include(c => c.Trainee)
                    .Include(c => c.Messages.OrderByDescending(m => m.SentAt).Take(1))
                    .FirstOrDefaultAsync(c =>
                        (c.TrainerId == currentProfile.Id && c.TraineeId == otherProfile.Id) ||
                        (c.TrainerId == otherProfile.Id && c.TraineeId == currentProfile.Id), ct);

                if (conversation == null)
                {
                    // Verify trainer-trainee relationship
                    var relationship = await _context.TrainerTrainees
                        .AnyAsync(tt =>
                            (tt.TrainerId == currentProfile.Id && tt.TraineeId == otherProfile.Id) ||
                            (tt.TrainerId == otherProfile.Id && tt.TraineeId == currentProfile.Id), ct);

                    if (!relationship)
                        return Error.Validation("No_Relationship", "No trainer-trainee relationship exists");

                    // Determine who is trainer
                    var trainerId = currentProfile.Role == UserRole.Trainer ? currentProfile.Id : otherProfile.Id;
                    var traineeId = currentProfile.Role == UserRole.User ? currentProfile.Id : otherProfile.Id;

                    var conversationResult = ChatConversation.Create(Guid.NewGuid(), trainerId, traineeId);
                    if (conversationResult.IsError)
                        return conversationResult.Errors;

                    conversation = conversationResult.Value;
                    _context.ChatConversations.Add(conversation);
                    await _context.SaveChangesAsync(ct);

                    // Reload with navigation properties
                    conversation = await _context.ChatConversations
                        .Include(c => c.Trainer)
                        .Include(c => c.Trainee)
                        .FirstAsync(c => c.Id == conversation.Id, ct);
                }

                // Get unread count
                var unreadCount = await _context.ChatMessages
                    .CountAsync(m => m.ConversationId == conversation.Id &&
                                    m.SenderId != currentProfile.Id && !m.IsRead, ct);

                return ChatConversationDto.FromEntity(conversation, currentProfile.Id, unreadCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting/creating conversation");
                return Error.Failure("Conversation_Error", "Failed to get or create conversation");
            }
        }

        public async Task<Result<List<ChatConversationDto>>> GetUserConversationsAsync(
            string userId, int pageSize = 20, int pageNumber = 1, CancellationToken ct = default)
        {
            try
            {
                var profile = await _context.ClientProfiles
                    .FirstOrDefaultAsync(p => p.AppUserId == userId, ct);

                if (profile == null)
                    return Error.NotFound("Profile_NotFound", "User profile not found");

                var conversations = await _context.ChatConversations
                    .Where(c => c.TrainerId == profile.Id || c.TraineeId == profile.Id)
                    .Include(c => c.Trainer)
                    .Include(c => c.Trainee)
                    .OrderByDescending(c => c.LastMessageAt ?? c.CreatedAtUtc)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync(ct);

                var result = new List<ChatConversationDto>();

                foreach (var conv in conversations)
                {
                    var unreadCount = await _context.ChatMessages
                        .CountAsync(m => m.ConversationId == conv.Id &&
                                        m.SenderId != profile.Id && !m.IsRead, ct);

                    result.Add(ChatConversationDto.FromEntity(conv, profile.Id, unreadCount));
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user conversations");
                return Error.Failure("Conversations_Error", "Failed to get conversations");
            }
        }

        public async Task<Result<List<ChatMessageDto>>> GetConversationMessagesAsync(
            string userId, Guid conversationId, int pageSize = 50, int pageNumber = 1, CancellationToken ct = default)
        {
            try
            {
                var profile = await _context.ClientProfiles
                    .FirstOrDefaultAsync(p => p.AppUserId == userId, ct);

                if (profile == null)
                    return Error.NotFound("Profile_NotFound", "User profile not found");

                // Verify user is part of conversation
                var conversation = await _context.ChatConversations
                    .FirstOrDefaultAsync(c => c.Id == conversationId &&
                        (c.TrainerId == profile.Id || c.TraineeId == profile.Id), ct);

                if (conversation == null)
                    return Error.NotFound("Conversation_NotFound", "Conversation not found");

                var messages = await _context.ChatMessages
                    .Where(m => m.ConversationId == conversationId)
                    .Include(m => m.Sender)
                    .OrderByDescending(m => m.SentAt)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync(ct);

                return messages.Select(ChatMessageDto.FromEntity).OrderBy(m => m.SentAt).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting conversation messages");
                return Error.Failure("Messages_Error", "Failed to get messages");
            }
        }

        public async Task<Result<int>> GetUnreadMessageCountAsync(
            string userId, Guid? conversationId = null, CancellationToken ct = default)
        {
            try
            {
                var profile = await _context.ClientProfiles
                    .FirstOrDefaultAsync(p => p.AppUserId == userId, ct);

                if (profile == null)
                    return Error.NotFound("Profile_NotFound", "User profile not found");

                var query = _context.ChatMessages
                    .Where(m => m.SenderId != profile.Id && !m.IsRead);

                if (conversationId.HasValue)
                {
                    query = query.Where(m => m.ConversationId == conversationId.Value);
                }
                else
                {
                    // Count across all user's conversations
                    var conversationIds = await _context.ChatConversations
                        .Where(c => c.TrainerId == profile.Id || c.TraineeId == profile.Id)
                        .Select(c => c.Id)
                        .ToListAsync(ct);

                    query = query.Where(m => conversationIds.Contains(m.ConversationId));
                }

                return await query.CountAsync(ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting unread count");
                return Error.Failure("Unread_Count_Error", "Failed to get unread count");
            }
        }

        public async Task<Result<ChatMessageDto>> SendMessageAsync(
            string userId, Guid conversationId, string content,
            MessageType type = MessageType.Text, IFormFile? attachment = null, CancellationToken ct = default)
        {
            try
            {
                var profile = await _context.ClientProfiles
                    .FirstOrDefaultAsync(p => p.AppUserId == userId, ct);

                if (profile == null)
                    return Error.NotFound("Profile_NotFound", "User profile not found");

                var conversation = await _context.ChatConversations
                    .FirstOrDefaultAsync(c => c.Id == conversationId &&
                        (c.TrainerId == profile.Id || c.TraineeId == profile.Id), ct);

                if (conversation == null)
                    return Error.NotFound("Conversation_NotFound", "Conversation not found");

                string? attachmentUrl = null;
                if (attachment != null && attachment.Length > 0)
                {
                    attachmentUrl = await SaveAttachmentAsync(attachment, type, ct);
                }

                var messageResult = ChatMessage.Create(
                    Guid.NewGuid(), conversationId, profile.Id,
                    content, type, attachmentUrl);

                if (messageResult.IsError)
                    return messageResult.Errors;

                var message = messageResult.Value;
                conversation.AddMessage(message);

                _context.ChatMessages.Add(message);
                await _context.SaveChangesAsync(ct);

                // Reload with sender info
                await _context.Entry(message).Reference(m => m.Sender).LoadAsync(ct);

                return ChatMessageDto.FromEntity(message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending message");
                return Error.Failure("Send_Message_Error", "Failed to send message");
            }
        }

        private async Task<string> SaveAttachmentAsync(IFormFile file, MessageType type, CancellationToken ct)
        {
            var folderName = type switch
            {
                MessageType.Image => "images",
                MessageType.Document => "documents",
                MessageType.Voice => "voice",
                MessageType.Video => "videos",
                _ => "attachments"
            };

            var uploadsFolder = Path.Combine(_environment.WebRootPath, "chat", folderName);
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream, ct);
            }

            const string baseUrl = "https://gymassistantapi.runasp.net";
            return $"{baseUrl}/chat/{folderName}/{uniqueFileName}";
        }
    }

}
