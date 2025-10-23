using GymAssistant_API.Model.Entities.Exercise;

namespace GymAssistant_API.Req_Res.Response.Exercise
{
    public sealed record WorkoutExerciseRes(
        Guid Id,
        Guid WorkoutSessionId,
        string SectionName,
        Guid? ExerciseId,
        string? ExerciseName,
        Guid? UserExerciseId,
        string? UserExerciseName,


        Guid ClientProfileId,
        IReadOnlyCollection<ExerciseSetRes> Sets
    )
    {
        public static WorkoutExerciseRes FromEntity(WorkoutExercise entity)
        {
            return new WorkoutExerciseRes(
                entity.Id,
                entity.WorkoutSessionId,
                entity.Exercise?.Section.Name ?? entity.UserExercise?.Section.Name ?? string.Empty,
                entity.ExerciseId,
                entity.Exercise?.Name,
                entity.UserExerciseId,
                entity.UserExercise?.Name,
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
