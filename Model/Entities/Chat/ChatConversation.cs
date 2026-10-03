using GymAssistant_API.Model.Entities.User;
using GymAssistant_API.Model.Results;

namespace GymAssistant_API.Model.Entities.Chat
{
    /// <summary>
    /// Chat Conversation between Trainer and Trainee
    /// </summary>
    public sealed class ChatConversation : Entity
    {
        private readonly List<ChatMessage> _messages = new();

        public Guid TrainerId { get; private set; }
        public ClientProfile Trainer { get; private set; } = default!;

        public Guid TraineeId { get; private set; }
        public ClientProfile Trainee { get; private set; } = default!;

        public DateTime? LastMessageAt { get; private set; }
        public string? LastMessageText { get; private set; }
        public bool IsActive { get; private set; } = true;

        public IReadOnlyCollection<ChatMessage> Messages => _messages.AsReadOnly();

        private ChatConversation() { }

        private ChatConversation(Guid id, Guid trainerId, Guid traineeId) : base(id)
        {
            TrainerId = trainerId;
            TraineeId = traineeId;
            CreatedAtUtc = DateTimeOffset.UtcNow;
        }

        public static Result<ChatConversation> Create(Guid id, Guid trainerId, Guid traineeId)
        {
            if (trainerId == Guid.Empty)
                return Error.Validation(Resources.LocalizationKeys.Trainer.TrainerIdRequired);

            if (traineeId == Guid.Empty)
                return Error.Validation(Resources.LocalizationKeys.Trainer.TraineeIdRequired);

            if (trainerId == traineeId)
                return Error.Validation(Resources.LocalizationKeys.Trainer.SameTrainerAndTrainee);

            return new ChatConversation(id, trainerId, traineeId);
        }

        public void AddMessage(ChatMessage message)
        {
            _messages.Add(message);
            LastMessageAt = message.SentAt;
            LastMessageText = message.Content;
        }

        public void Archive() => IsActive = false;
        public void Activate() => IsActive = true;
    }
}
