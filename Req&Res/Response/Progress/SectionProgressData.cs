namespace GymAssistant_API.Req_Res.Response.Progress
{
    public class SectionProgressData
    {
        public Guid? ExerciseId { get; set; } = null;
        public string? ExerciseName { get; set; } = null;
        public Guid? customExerciseId { get; set; } = null;
        public string? customExerciseName { get; set; } = null;
        public decimal TotalVolume { get; set; }
        public int TotalSets { get; set; }
        public decimal MaxWeight { get; set; }
        public int SessionsCount { get; set; }
        public DateTimeOffset LastWorkoutDate { get; set; }
    }
}
