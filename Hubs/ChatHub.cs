using GymAssistant_API.Data;
using GymAssistant_API.Model.Entities.Chat;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;
using System.Security.Claims;

namespace GymAssistant_API.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ChatHub> _logger;

        // Track online users and their connection IDs
        private static readonly ConcurrentDictionary<string, HashSet<string>> UserConnections = new();

        // Track typing indicators
        private static readonly ConcurrentDictionary<Guid, List<TypingIndicator>> TypingIndicators = new();

        public ChatHub(AppDbContext context, ILogger<ChatHub> logger)
        {
            _context = context;
            _logger = logger;
        }

        private string GetCurrentUserId()
        {
            return Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                   ?? throw new HubException("User not authenticated");
        }

        private async Task<Guid> GetUserProfileId(string userId)
        {
            var profile = await _context.ClientProfiles
                .FirstOrDefaultAsync(p => p.AppUserId == userId);

            if (profile == null)
                throw new HubException("User profile not found");

            return profile.Id;
        }

        #region Connection Management

        public override async Task OnConnectedAsync()
        {
            var userId = GetCurrentUserId();
            var connectionId = Context.ConnectionId;

            // Add user connection
            UserConnections.AddOrUpdate(
                userId,
                new HashSet<string> { connectionId },
                (key, existing) =>
                {
                    existing.Add(connectionId);
                    return existing;
                });

            _logger.LogInformation("User {UserId} connected with connection {ConnectionId}",
                userId, connectionId);

            // Join user's conversation groups
            var profileId = await GetUserProfileId(userId);
            var conversations = await _context.ChatConversations
                .Where(c => c.TrainerId == profileId || c.TraineeId == profileId)
                .Select(c => c.Id)
                .ToListAsync();

            foreach (var conversationId in conversations)
            {
                await Groups.AddToGroupAsync(connectionId, $"conversation_{conversationId}");
            }

            // Notify others that user is online
            await Clients.Others.SendAsync("UserOnline", userId);

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = GetCurrentUserId();
            var connectionId = Context.ConnectionId;

            // Remove connection
            if (UserConnections.TryGetValue(userId, out var connections))
            {
                connections.Remove(connectionId);
                if (connections.Count == 0)
                {
                    UserConnections.TryRemove(userId, out _);
                    // Notify others that user is offline
                    await Clients.Others.SendAsync("UserOffline", userId);
                }
            }

            _logger.LogInformation("User {UserId} disconnected", userId);

            await base.OnDisconnectedAsync(exception);
        }

        #endregion

        #region Send Messages

        public async Task SendMessage(Guid conversationId, string content,
                                     MessageType type = MessageType.Text,
                                     string? attachmentUrl = null)
        {
            try
            {
                var userId = GetCurrentUserId();
                var senderProfileId = await GetUserProfileId(userId);

                // Verify conversation exists and user is part of it
                var conversation = await _context.ChatConversations
                    .Include(c => c.Trainer)
                    .Include(c => c.Trainee)
                    .FirstOrDefaultAsync(c => c.Id == conversationId);

                if (conversation == null)
                    throw new HubException("Conversation not found");

                if (conversation.TrainerId != senderProfileId &&
                    conversation.TraineeId != senderProfileId)
                    throw new HubException("You are not part of this conversation");

                // Create message
                var messageResult = ChatMessage.Create(
                    Guid.NewGuid(),
                    conversationId,
                    senderProfileId,
                    content,
                    type,
                    attachmentUrl
                );

                if (messageResult.IsError)
                    throw new HubException(messageResult.TopError.Description);

                var message = messageResult.Value;
                conversation.AddMessage(message);

                _context.ChatMessages.Add(message);
                await _context.SaveChangesAsync();

                // Get sender info
                var sender = await _context.ClientProfiles
                    .FirstOrDefaultAsync(p => p.Id == senderProfileId);

                // Prepare response
                var messageDto = new
                {
                    message.Id,
                    message.ConversationId,
                    SenderId = senderProfileId,
                    SenderName = sender?.FullName,
                    message.Content,
                    message.Type,
                    message.AttachmentUrl,
                    message.SentAt,
                    message.IsRead,
                    message.IsEdited
                };

                // Send to conversation group
                await Clients.Group($"conversation_{conversationId}")
                    .SendAsync("ReceiveMessage", messageDto);

                // Clear typing indicator
                await StopTyping(conversationId);

                _logger.LogInformation("Message sent in conversation {ConversationId}", conversationId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending message");
                throw new HubException("Failed to send message: " + ex.Message);
            }
        }

        #endregion

        #region Mark as Read

        public async Task MarkAsRead(Guid messageId)
        {
            try
            {
                var userId = GetCurrentUserId();
                var profileId = await GetUserProfileId(userId);

                var message = await _context.ChatMessages
                    .Include(m => m.Conversation)
                    .FirstOrDefaultAsync(m => m.Id == messageId);

                if (message == null)
                    throw new HubException("Message not found");

                // Verify user is recipient
                if (message.SenderId == profileId)
                    return; // Sender doesn't need to mark as read

                if (message.Conversation.TrainerId != profileId &&
                    message.Conversation.TraineeId != profileId)
                    throw new HubException("You are not part of this conversation");

                if (!message.IsRead)
                {
                    message.MarkAsRead();
                    await _context.SaveChangesAsync();

                    // Notify sender that message was read
                    await Clients.Group($"conversation_{message.ConversationId}")
                        .SendAsync("MessageRead", new { messageId, readAt = message.ReadAt });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking message as read");
                throw new HubException("Failed to mark message as read");
            }
        }

        public async Task MarkConversationAsRead(Guid conversationId)
        {
            try
            {
                var userId = GetCurrentUserId();
                var profileId = await GetUserProfileId(userId);

                var unreadMessages = await _context.ChatMessages
                    .Where(m => m.ConversationId == conversationId &&
                               m.SenderId != profileId &&
                               !m.IsRead)
                    .ToListAsync();

                foreach (var message in unreadMessages)
                {
                    message.MarkAsRead();
                }

                await _context.SaveChangesAsync();

                // Notify other party
                await Clients.Group($"conversation_{conversationId}")
                    .SendAsync("ConversationRead", conversationId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking conversation as read");
                throw new HubException("Failed to mark conversation as read");
            }
        }

        #endregion

        #region Typing Indicators

        public async Task StartTyping(Guid conversationId)
        {
            try
            {
                var userId = GetCurrentUserId();
                var profileId = await GetUserProfileId(userId);

                var profile = await _context.ClientProfiles
                    .FirstOrDefaultAsync(p => p.Id == profileId);

                var indicator = new TypingIndicator
                {
                    ConversationId = conversationId,
                    UserId = profileId,
                    UserName = profile?.FullName ?? "Unknown"
                };

                TypingIndicators.AddOrUpdate(
                    conversationId,
                    new List<TypingIndicator> { indicator },
                    (key, existing) =>
                    {
                        existing.RemoveAll(i => i.UserId == profileId);
                        existing.Add(indicator);
                        return existing;
                    });

                // Notify others in conversation (exclude sender)
                await Clients.GroupExcept($"conversation_{conversationId}", Context.ConnectionId)
                    .SendAsync("UserTyping", new { conversationId, userId = profileId, userName = indicator.UserName });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in StartTyping");
            }
        }

        public async Task StopTyping(Guid conversationId)
        {
            try
            {
                var userId = GetCurrentUserId();
                var profileId = await GetUserProfileId(userId);

                if (TypingIndicators.TryGetValue(conversationId, out var indicators))
                {
                    indicators.RemoveAll(i => i.UserId == profileId);
                    if (indicators.Count == 0)
                    {
                        TypingIndicators.TryRemove(conversationId, out _);
                    }
                }

                // Notify others in conversation
                await Clients.GroupExcept($"conversation_{conversationId}", Context.ConnectionId)
                    .SendAsync("UserStoppedTyping", new { conversationId, userId = profileId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in StopTyping");
            }
        }

        #endregion

        #region Edit/Delete Messages

        public async Task EditMessage(Guid messageId, string newContent)
        {
            try
            {
                var userId = GetCurrentUserId();
                var profileId = await GetUserProfileId(userId);

                var message = await _context.ChatMessages
                    .FirstOrDefaultAsync(m => m.Id == messageId && m.SenderId == profileId);

                if (message == null)
                    throw new HubException("Message not found or you are not the sender");

                var editResult = message.Edit(newContent);
                if (editResult.IsError)
                    throw new HubException(editResult.TopError.Description);

                await _context.SaveChangesAsync();

                // Notify conversation
                await Clients.Group($"conversation_{message.ConversationId}")
                    .SendAsync("MessageEdited", new
                    {
                        messageId,
                        newContent,
                        editedAt = message.EditedAt
                    });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error editing message");
                throw new HubException("Failed to edit message");
            }
        }

        public async Task DeleteMessage(Guid messageId)
        {
            try
            {
                var userId = GetCurrentUserId();
                var profileId = await GetUserProfileId(userId);

                var message = await _context.ChatMessages
                    .FirstOrDefaultAsync(m => m.Id == messageId && m.SenderId == profileId);

                if (message == null)
                    throw new HubException("Message not found or you are not the sender");

                var conversationId = message.ConversationId;
                _context.ChatMessages.Remove(message);
                await _context.SaveChangesAsync();

                // Notify conversation
                await Clients.Group($"conversation_{conversationId}")
                    .SendAsync("MessageDeleted", messageId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting message");
                throw new HubException("Failed to delete message");
            }
        }

        #endregion

        #region Helper Methods

        public static bool IsUserOnline(string userId)
        {
            return UserConnections.ContainsKey(userId);
        }

        public static int GetOnlineUsersCount()
        {
            return UserConnections.Count;
        }

        #endregion
    }
}
