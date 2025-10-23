namespace GymAssistant_API.Req_Res.Response.Records
{
    public class Milestone
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public PersonalRecordResponse? Record { get; set; }
        public DateTimeOffset Date { get; set; }
    }
}
