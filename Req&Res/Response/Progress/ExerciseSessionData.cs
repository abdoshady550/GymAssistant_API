namespace GymAssistant_API.Req_Res.Response.Progress
{
    public class ExerciseSessionData
    {
        public DateTime Date { get; set; }
        public List<SetProgressData> Sets { get; set; } = new();
        public decimal MaxWeight { get; set; }
        public decimal TotalVolume { get; set; }
        public int TotalSets { get; set; }
    }
}
