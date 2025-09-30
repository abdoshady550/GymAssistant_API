namespace GymAssistant_API.Req_Res.Response.Progress
{
    public class ExerciseProgressData
    {
        public Guid ExerciseId { get; set; }
        public string ExerciseName { get; set; } = string.Empty;
        public string SectionName { get; set; } = string.Empty;
        public List<ExerciseSessionData> Sessions { get; set; } = new();
    }
}
