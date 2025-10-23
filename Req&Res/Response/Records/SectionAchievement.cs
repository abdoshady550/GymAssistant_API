namespace GymAssistant_API.Req_Res.Response.Records
{
    public class SectionAchievement
    {
        public string? SectionName { get; set; } = string.Empty;
        public int? RecordCount { get; set; }
        public DateTimeOffset? LatestRecord { get; set; }
    }
}
