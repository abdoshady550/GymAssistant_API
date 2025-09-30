namespace GymAssistant_API.Req_Res.Response.Progress
{
    public class ExerciseChartData
    {
        public List<ChartPoint> WeightProgression { get; set; } = new();
        public List<ChartPoint> VolumeProgression { get; set; } = new();
        public List<ChartPoint> RepsProgression { get; set; } = new();
    }
}
