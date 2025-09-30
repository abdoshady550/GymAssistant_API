using GymAssistant_API.Model.Entities.User;

namespace GymAssistant_API.Req_Res.Response.Trainer
{
    public sealed class TrainerTraineeResponse
    {
        public Guid Id { get; set; }
        public Guid TrainerId { get; set; }
        public Guid TraineeId { get; set; }
        public string TrainerFullName { get; set; } = string.Empty;
        public string TraineeFullName { get; set; } = string.Empty;
        public DateTimeOffset AssignedDate { get; set; }

        public static TrainerTraineeResponse FromEntity(TrainerTrainee entity)
        {
            return new TrainerTraineeResponse
            {
                Id = entity.Id,
                TrainerId = entity.TrainerId,
                TraineeId = entity.TraineeId,
                TrainerFullName = entity.Trainer?.FullName ?? string.Empty,
                TraineeFullName = entity.Trainee?.FullName ?? string.Empty,
                AssignedDate = entity.CreatedAtUtc
            };
        }
    }
}
