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
        public static ExerciseResponse FromEntity(ExerciseEntity exercise, GymAssistant_API.Resources.ISeedDataLocalizer? seedLocalizer = null)
        {
            var sectionName = seedLocalizer != null && exercise.Section != null
                ? seedLocalizer.SectionName(exercise.SectionId, exercise.Section.Name)
                : exercise.Section?.Name ?? string.Empty;

            var name = seedLocalizer != null
                ? seedLocalizer.ExerciseName(exercise.Id, exercise.Name)
                : exercise.Name;

            var description = seedLocalizer != null
                ? seedLocalizer.ExerciseDescription(exercise.Id, exercise.Description)
                : exercise.Description;

            var instructions = seedLocalizer != null
                ? seedLocalizer.ExerciseInstructions(exercise.Id, exercise.Instructions)
                : exercise.Instructions;

            var equipment = seedLocalizer != null
                ? seedLocalizer.ExerciseEquipment(exercise.Id, exercise.Equipment)
                : exercise.Equipment;

            return new ExerciseResponse(
                exercise.Id,
                exercise.SectionId,
                sectionName,
                name,
                description,
                instructions,
                equipment,
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
