using GymAssistant_API.Model.Entities.Exercise;
using GymAssistant_API.Model.Results;
using GymAssistant_API.Req_Res.Response;
using GymAssistant_API.Req_Res.Response.Exercise;

namespace GymAssistant_API.Repository.Interfaces.ExerciseExercises
{
    public interface IExercise
    {
        Task<Result<List<SectionResponse>>> GetSectionsAsync(CancellationToken ct = default);
        Task<Result<Deleted>> DeleteExerciseAsync(Guid id, CancellationToken ct = default);
        Task<Result<Updated>> UpdateExerciseAsync(Guid id, Guid sectionId, string name, string? description, string? instructions, IFormFile? imageFile, string? equipment, DifficultyLevel? difficultyLevel, int? defaultSets, int? defaultReps, CancellationToken ct = default);
        Task<Result<ExerciseResponse>> CreateExerciseAsync(Guid sectionId, string name, string? description = null, string? instructions = null, IFormFile? imageFile = null, string? equipment = null, DifficultyLevel? difficultyLevel = null, int? defaultSets = null, int? defaultReps = null, CancellationToken ct = default);
        Task<Result<List<ExerciseResponse>>> GetExercisesBySectionAsync(Guid sectionId, DifficultyLevel? difficulty = null, CancellationToken ct = default);
        Task<Result<ExerciseResponse>> GetExerciseAsync(Guid exerciseId, CancellationToken ct = default);
        Task<Result<CustomExerciseRes>> CreateCustomExerciseAsync(string userId, string name, string? description = null, string? Instructions = null, string? Equipment = null, IFormFile? ImageFile = default, CancellationToken ct = default);
        Task<Result<List<CustomExerciseRes>>> GetCustomExercisesAsync(string userId, CancellationToken ct = default);
        Task<Result<CustomExerciseRes>> GetCustomExerciseAsync(string userId, Guid exerciseId, CancellationToken ct = default);
        Task<Result<Updated>> UpdateCustomExerciseAsync(string userId, Guid exerciseId, string name, string? description = null, string? Instructions = null, string? Equipment = null, IFormFile? imageFile = default, CancellationToken ct = default);
        Task<Result<Deleted>> DeleteCustomExerciseAsync(string userId, Guid exerciseId, CancellationToken ct = default);
    }
}
