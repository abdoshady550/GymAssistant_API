using GymAssistant_API.Model.Results;

namespace GymAssistant_API.Model.Entities.User
{
    public sealed class TrainerRequest : Entity
    {
        public Guid TrainerId { get; private set; }
        public Guid TraineeId { get; private set; }
        public RequestStatus Status { get; private set; }
        public string? Message { get; private set; }
        public DateTimeOffset? RespondedAtUtc { get; private set; }

        public ClientProfile Trainer { get; private set; } = default!;
        public ClientProfile Trainee { get; private set; } = default!;

        private TrainerRequest() { }

        private TrainerRequest(Guid id, Guid trainerId, Guid traineeId, string? message = null)
            : base(id)
        {
            TrainerId = trainerId;
            TraineeId = traineeId;
            Status = RequestStatus.Pending;
            Message = message;
            CreatedAtUtc = DateTimeOffset.UtcNow;
        }

        public static Result<TrainerRequest> Create(Guid id,
                                                    Guid trainerId,
                                                    Guid traineeId,
                                                    string? message = null)
        {
            if (trainerId == Guid.Empty)
            {
                return UserErrors.IdRequired;
            }
            if (traineeId == Guid.Empty)
            {
                return UserErrors.IdRequired;
            }
            if (trainerId == traineeId)
            {
                return TrainerRequestErrors.SameTrainerAndTrainee;
            }
            return new TrainerRequest(id, trainerId, traineeId, message);
        }

        public Result<Updated> Accept()
        {
            if (Status != RequestStatus.Pending)
            {
                return TrainerRequestErrors.RequestNotPending;
            }
            Status = RequestStatus.Accepted;
            RespondedAtUtc = DateTimeOffset.UtcNow;
            return Result.Updated;
        }

        public Result<Updated> Reject()
        {
            if (Status != RequestStatus.Pending)
            {
                return TrainerRequestErrors.RequestNotPending;
            }
            Status = RequestStatus.Rejected;
            RespondedAtUtc = DateTimeOffset.UtcNow;
            return Result.Updated;
        }

        public Result<Updated> Cancel()
        {
            if (Status != RequestStatus.Pending)
            {
                return TrainerRequestErrors.RequestNotPending;
            }
            Status = RequestStatus.Cancelled;
            RespondedAtUtc = DateTimeOffset.UtcNow;
            return Result.Updated;
        }
    }

    public enum RequestStatus
    {
        Pending = 1,
        Accepted = 2,
        Rejected = 3,
        Cancelled = 4
    }
}
