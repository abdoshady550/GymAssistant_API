namespace GymAssistant_API.Req_Res.Response.Progress
{
    public class ProgressOverviewData
    {
        public int TotalWorkouts { get; set; }
        public int TotalSets { get; set; }
        public decimal TotalVolume { get; set; }
        public int AverageDuration { get; set; }
        public int PersonalRecords { get; set; }
        public string? MostActiveDay { get; set; }
        public double WorkoutFrequency { get; set; }
    }
}
