namespace GymAssistant_API.Req_Res.Response.Progress
{
    public class SectionProgressData
    {
        public Guid ExerciseId { get; set; }
        public string ExerciseName { get; set; } = string.Empty;
        public decimal TotalVolume { get; set; }
        public int TotalSets { get; set; }
        public decimal MaxWeight { get; set; }
        public int SessionsCount { get; set; }
        public DateTimeOffset LastWorkoutDate { get; set; }
    }
}
