using GymAssistant_API.Model.Identity;
using GymAssistant_API.Model.Results;

namespace GymAssistant_API.Model.Entities.User
{
    public sealed class TrainerTrainee : Entity
    {

        public Guid TrainerId { get; private set; }
        public Guid TraineeId { get; private set; }


        public ClientProfile Trainer { get; private set; } = default!;
        public ClientProfile Trainee { get; private set; } = default!;

        private TrainerTrainee() { }

        private TrainerTrainee(Guid id, Guid trainerId, Guid traineeId) : base(id)
        {
            TrainerId = trainerId;
            TraineeId = traineeId;
            CreatedAtUtc = DateTimeOffset.UtcNow;
        }
        public static Result<TrainerTrainee> Create(Guid id, Guid trainerId, Guid traineeId)
        {
            if (trainerId == Guid.Empty)
            {
                return UserErrors.IdRequired;
            }
            if (traineeId == Guid.Empty)
            {
                return UserErrors.IdRequired;
            }
            return new TrainerTrainee(id, trainerId, traineeId);
        }
    }
}
