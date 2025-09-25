using GymAssistant_API.Model.Entities.Exercise;

namespace GymAssistant_API.Req_Res.Reqeust.Exercise
{
    public record ExerciseReq(
      string Name,
      string? Description = null,
      string? Instructions = null,
      IFormFile? ImageFile = null,
      string? Equipment = null,
      DifficultyLevel? DifficultyLevel = null,
      int? DefaultSets = null,
      int? DefaultReps = null
  );
    public record UpdateExerciseReq(
    string? Name,
    string? Description,
    string? Instructions,
    IFormFile? ImageFile,
    string? Equipment,
    DifficultyLevel? DifficultyLevel,
    int? DefaultSets,
    int? DefaultReps
 );

}
