using GymAssistant_API.Model.Entities.User;

namespace GymAssistant_API.Req_Res.Response.Trainer
{
    public class TrainerRequestResponse
    {
        public Guid RequestId { get; set; }
        public Guid TrainerId { get; set; }
        public string? TrainerFullName { get; set; } = null;
        public string? TrainerImage { get; set; } = null;

        public Guid TraineeId { get; set; }
        public string? TraineeFullName { get; set; } = null;
        public string? TraineeImage { get; set; } = null;
        public RequestStatus Status { get; set; }
        public string? Message { get; set; }
        public DateTimeOffset CreatedAtUtc { get; set; }
        public DateTimeOffset? RespondedAtUtc { get; set; }

        public static TrainerRequestResponse FromEntity(TrainerRequest request)
        {
            return new TrainerRequestResponse
            {
                RequestId = request.Id,
                TrainerId = request.TrainerId,
                TrainerFullName = request.Trainer?.FullName ?? string.Empty,
                TraineeId = request.TraineeId,
                TraineeFullName = request.Trainee?.FullName ?? string.Empty,
                TraineeImage = request.Trainee?.Image ?? null,
                Status = request.Status,
                Message = request.Message,
                CreatedAtUtc = request.CreatedAtUtc,
                RespondedAtUtc = request.RespondedAtUtc
            };
        }

        public static TrainerRequestResponse FromEntity(UserRequest request)
        {
            return new TrainerRequestResponse
            {
                RequestId = request.Id,
                TrainerId = request.TrainerId,
                TrainerFullName = request.Trainer?.FullName ?? string.Empty,
                TrainerImage = request.Trainer?.Image ?? null,
                TraineeId = request.TraineeId,
                TraineeFullName = request.Trainee?.FullName ?? string.Empty,
                Status = request.Status,
                Message = request.Message,
                CreatedAtUtc = request.CreatedAtUtc,
                RespondedAtUtc = request.RespondedAtUtc
            };
        }
    }

    public class TrainerRequestListResponse
    {
        public int TotalCount { get; set; }
        public int PendingCount { get; set; }
        public List<TrainerRequestResponse> Requests { get; set; } = new();
    }


}
