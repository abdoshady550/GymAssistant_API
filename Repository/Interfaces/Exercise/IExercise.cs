using GymAssistant_API.Model.Entities.Exercise;
using GymAssistant_API.Model.Results;

namespace GymAssistant_API.Repository.Interfaces.ExerciseExercise
{
    public interface IExercise
    {
        Task<Result<List<Section>>> GetSectionsAsync(CancellationToken ct = default);
        Task<Result<List<Exercise>>> GetExercisesBySectionAsync(Guid sectionId, DifficultyLevel? difficulty = null, CancellationToken ct = default);
        Task<Result<Exercise>> GetExerciseAsync(Guid exerciseId, CancellationToken ct = default);
        Task<Result<UserExercise>> CreateCustomExerciseAsync(string userId, string name, string? description = null, CancellationToken ct = default);
        Task<Result<List<UserExercise>>> GetCustomExercisesAsync(string userId, CancellationToken ct = default);
        Task<Result<UserExercise>> GetCustomExerciseAsync(string userId, Guid exerciseId, CancellationToken ct = default);
        Task<Result<Updated>> UpdateCustomExerciseAsync(string userId, Guid exerciseId, string name, string? description = null, string? Instructions = null, string? Equipment = null, string? ImageUrl = null, CancellationToken ct = default);
        Task<Result<Deleted>> DeleteCustomExerciseAsync(string userId, Guid exerciseId, CancellationToken ct = default);
    }
}
