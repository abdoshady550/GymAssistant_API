using GymAssistant_API.Model.Entities.Exercise;

namespace GymAssistant_API.Req_Res.Response.Exercise
{
    public sealed record WorkoutExerciseRes(
        Guid Id,
        Guid WorkoutSessionId,
        Guid? ExerciseId,
        Guid? UserExerciseId,
        Guid ClientProfileId,
        IReadOnlyCollection<ExerciseSetRes> Sets
    )
    {
        public static WorkoutExerciseRes FromEntity(WorkoutExercise entity)
        {
            return new WorkoutExerciseRes(
                entity.Id,
                entity.WorkoutSessionId,
                entity.ExerciseId,
                entity.UserExerciseId,
                entity.ClientProfileId,
                entity.Sets.Select(ExerciseSetRes.FromEntity).ToList()
            );
        }
    };
    public sealed record ExerciseSetRes(
        Guid Id,
        Guid WorkoutExerciseId,
        int SetNumber,
        int Reps,
        decimal WeightKg,
        int? RestTimeSeconds,
        bool IsCompleted,
        bool IsPersonalRecord,
        string? Notes,
        DateTimeOffset CreatedAtUtc
        )
    {
        public static ExerciseSetRes FromEntity(ExerciseSet set)
        {
            return new ExerciseSetRes(
                set.Id,
                set.WorkoutExerciseId,
                set.SetNumber,
                set.Reps,
                set.WeightKg,
                set.RestTimeSeconds,
                set.IsCompleted,
                set.IsPersonalRecord,
                set.Notes,
                set.CreatedAtUtc
            );
        }
    };
}
