namespace GymAssistant_API.Req_Res.Response.Progress
{
    public class VolumeChartData
    {
        public List<ChartPoint> DailyVolume { get; set; } = new();
        public List<ChartPoint> WeeklyVolume { get; set; } = new();
        public decimal TotalVolume { get; set; }
        public decimal AverageDaily { get; set; }
    }
}
