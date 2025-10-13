using GymAssistant_API.Model.Entities.Exercise;

namespace GymAssistant_API.Req_Res.Response
{
    public record CustomExerciseRes(
      Guid Id,
      string UserId,
      Guid sectionId,
      string Name,
      string? Description = default,
      string? Instructions = default,
      string? Equipment = default,
      string? ImageUrl = default,
      bool IsCustomExercise = true,
      DateTimeOffset? CreatedAtUtc = default,
      DifficultyLevel? DifficultyLevel = default
  )
    {
        public static CustomExerciseRes FromEntity(UserExercise exercise)
        {
            return new CustomExerciseRes(
                exercise.Id,
                exercise.UserId,
                exercise.SectionId,
                exercise.Name,
                exercise.Description,
                exercise.Instructions,
                exercise.Equipment,
                exercise.ImageUrl,
                exercise.IsCustomExercise,
                exercise.CreatedAtUtc,
                exercise.DifficultyLevel
                );
        }
    };
}
