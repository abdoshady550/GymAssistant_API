using GymAssistant_API.Model.Entities.Exercise;

namespace GymAssistant_API.Req_Res.Response
{
    public record ExerciseResponse(
        Guid Id,
        Guid SectionId,
        string Name,
        string? Description = default,
        string? Instructions = default,
        string? Equipment = default,
        string? ImageUrl = default,
        DifficultyLevel? DifficultyLevel = default,
        int? DefaultSets = default,
        int? DefaultReps = default,
        DateTimeOffset? CreatedAtUtc = default
    );
}
