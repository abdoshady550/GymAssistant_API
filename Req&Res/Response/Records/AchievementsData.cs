using GymAssistant_API.Model.Entities.Exercise;

namespace GymAssistant_API.Req_Res.Response.Records
{
    public class AchievementsData
    {
        public int TotalRecords { get; set; }
        public int WeightRecords { get; set; }
        public int RepsRecords { get; set; }
        public int VolumeRecords { get; set; }
        public List<PersonalRecordResponse> RecentRecords { get; set; } = new();
        public List<SectionAchievement> RecordsBySection { get; set; } = new();
        public List<Milestone> Milestones { get; set; } = new();
    }
}
