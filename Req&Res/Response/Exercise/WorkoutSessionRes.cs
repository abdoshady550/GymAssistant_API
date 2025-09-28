using GymAssistant_API.Model.Entities.Exercise;

namespace GymAssistant_API.Req_Res.Response.Exercise
{
    public sealed record WorkoutSessionRes(
         Guid Id,
         Guid ClientProfileId,
         Guid? CreatedByTrainerId,
         DateTime Date,
         DateTime? StartTime,
         DateTime? EndTime,
         bool IsCompleted,
         int? DurationMinutes,
         string? Notes,
         IReadOnlyCollection<WorkoutExerciseRes> WorkoutExercises
     )
    {
        public static WorkoutSessionRes FromEntity(WorkoutSession session)
        {
            return new WorkoutSessionRes(
                session.Id,
                session.ClientProfileId,
                session.CreatedByTrainerId,
                session.Date,
                session.StartTime,
                session.EndTime,
                session.IsCompleted,
                session.DurationMinutes,
                session.Notes,
                session.WorkoutExercises.Select(WorkoutExerciseRes.FromEntity).ToList()
            );
        }
    }
}
