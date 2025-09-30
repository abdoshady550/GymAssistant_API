using GymAssistant_API.Model.Entities.Exercise;
using GymAssistant_API.Req_Res.Response.Exercise;

namespace GymAssistant_API.Req_Res.Response.Trainer
{
    public class TrainerDashboardData
    {
        public int TotalTrainees { get; set; }
        public int ActiveTraineesToday { get; set; }
        public int TotalSessionsCreated { get; set; }
        public int SessionsThisWeek { get; set; }
        public List<TraineeData> RecentlyActiveTrainees { get; set; } = new();
        public List<WorkoutSessionRes> RecentSessions { get; set; } = new();
        public Dictionary<string, int> TraineesBySection { get; set; } = new();
    }
}
