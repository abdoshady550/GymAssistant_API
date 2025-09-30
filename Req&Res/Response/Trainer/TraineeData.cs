using GymAssistant_API.Model.Entities.User;

namespace GymAssistant_API.Req_Res.Response.Trainer
{
    public class TraineeData
    {
        public Guid TraineeId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public Gender Gender { get; set; }
        public int? Age { get; set; }
        public int? HeightCm { get; set; }
        public decimal? CurrentWeight { get; set; }
        public int TotalWorkouts { get; set; }
        public DateTime? LastWorkout { get; set; }
        public int PersonalRecords { get; set; }
        public DateTimeOffset AssignedDate { get; set; }
    }
}
