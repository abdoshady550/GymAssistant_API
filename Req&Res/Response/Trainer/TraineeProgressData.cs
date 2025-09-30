using GymAssistant_API.Model.Entities.Exercise;
using GymAssistant_API.Req_Res.Response.Exercise;
using GymAssistant_API.Req_Res.Response.Progress;
using GymAssistant_API.Req_Res.Response.Records;

namespace GymAssistant_API.Req_Res.Response.Trainer
{
    public class TraineeProgressData
    {
        public TraineeData TraineeInfo { get; set; } = new();
        public ProgressOverviewData ProgressOverview { get; set; } = new();
        public List<PersonalRecordResponse> RecentRecords { get; set; } = new();
        public List<WorkoutSessionRes> RecentWorkouts { get; set; } = new();
        public List<SectionProgressData> SectionProgress { get; set; } = new();
    }
}
