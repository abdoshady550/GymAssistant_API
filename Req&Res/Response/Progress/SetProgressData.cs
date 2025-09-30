namespace GymAssistant_API.Req_Res.Response.Progress
{
    public class SetProgressData
    {
        public int SetNumber { get; set; }
        public int Reps { get; set; }
        public decimal WeightKg { get; set; }
        public decimal Volume { get; set; }
        public bool IsPersonalRecord { get; set; }
    }
}
