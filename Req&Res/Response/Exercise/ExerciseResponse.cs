using ExerciseEntity = GymAssistant_API.Model.Entities.Exercise.Exercise;
using GymAssistant_API.Model.Entities.Exercise;

namespace GymAssistant_API.Req_Res.Response
{
    public record ExerciseResponse(
        Guid Id,
        Guid SectionId,
        string SectionName,
        string Name,
        string? Description = default,
        string? Instructions = default,
        string? Equipment = default,
        string? ImageUrl = default,
        DifficultyLevel? DifficultyLevel = default,
        int? DefaultSets = default,
        int? DefaultReps = default,
        DateTimeOffset? CreatedAtUtc = default,
        bool? IsCustomExercise = false
    )
    {
        public static ExerciseResponse FromEntity(ExerciseEntity exercise)
        {
            return new ExerciseResponse(
                exercise.Id,
                exercise.SectionId,
                exercise.Section.Name,
                exercise.Name,
                exercise.Description,
                exercise.Instructions,
                exercise.Equipment,
                exercise.ImageUrl,
                exercise.DifficultyLevel,
                exercise.DefaultSets,
                exercise.DefaultReps,
                exercise.CreatedAtUtc,
                exercise.IsCustomExercise
                );
        }
    };
}
