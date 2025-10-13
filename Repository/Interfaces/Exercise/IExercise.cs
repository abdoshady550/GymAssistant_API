using GymAssistant_API.Model.Entities.Exercise;
using GymAssistant_API.Model.Results;
using GymAssistant_API.Req_Res.Response;
using GymAssistant_API.Req_Res.Response.Exercise;

namespace GymAssistant_API.Repository.Interfaces.ExerciseExercises
{
    public interface IExercise
    {
        Task<Result<List<SectionResponse>>> GetSectionsAsync(CancellationToken ct = default);
        Task<Result<SectionResponse>> GetSectionByIdAsync(string userId, Guid sectionId, CancellationToken ct = default);
        Task<Result<SectionGroupResponse>> CreateSectionGroup(string userId, Guid sectionId, string name, string descripion, CancellationToken ct = default);
        Task<Result<List<SectionGroupResponse>>> AllSectionGroups(string userId, Guid sectionId, CancellationToken ct = default);
        Task<Result<SectionGroupResponse>> AddExerciseToGroup(string userId, Guid groupId, Guid? exerciseId, Guid? customExerciseId, CancellationToken ct = default);
        Task<Result<Updated>> UpdateGroup(string userId, Guid groupId, string name, string descripion, CancellationToken ct = default);
        Task<Result<Deleted>> DeleteGroup(string userId, Guid groupId, CancellationToken ct = default);
        Task<Result<Deleted>> DeleteExerciseFromGroup(string userId, Guid groupId, Guid? exerciseId, Guid? customExerciseId,
                                                                           CancellationToken ct = default);
        Task<Result<Deleted>> DeleteExerciseAsync(Guid id, CancellationToken ct = default);
        Task<Result<Updated>> UpdateExerciseAsync(Guid id, Guid sectionId, string name, string? description, string? instructions, IFormFile? imageFile, string? equipment, DifficultyLevel? difficultyLevel, int? defaultSets, int? defaultReps, CancellationToken ct = default);
        Task<Result<ExerciseResponse>> CreateExerciseAsync(Guid sectionId, string name, string? description = null, string? instructions = null, IFormFile? imageFile = null, string? equipment = null, DifficultyLevel? difficultyLevel = null, int? defaultSets = null, int? defaultReps = null, CancellationToken ct = default);
        Task<Result<ExercisesResponse>> GetExercisesBySectionAsync(string userId, Guid sectionId, string? searchTerm = null, DifficultyLevel? difficulty = null, CancellationToken ct = default);
        Task<Result<ExerciseResponse>> GetExerciseAsync(Guid exerciseId, CancellationToken ct = default);
        Task<Result<CustomExerciseRes>> CreateCustomExerciseAsync(string userId, Guid sectionId, string name, string? description = null, string? Instructions = null, string? Equipment = null, IFormFile? ImageFile = default, DifficultyLevel? DifficultyLevel = null, CancellationToken ct = default);
        Task<Result<List<CustomExerciseRes>>> GetCustomExercisesAsync(string userId, DifficultyLevel? difficulty = null, CancellationToken ct = default);
        Task<Result<CustomExerciseRes>> GetCustomExerciseAsync(string userId, Guid exerciseId, CancellationToken ct = default);
        Task<Result<Updated>> UpdateCustomExerciseAsync(string userId, Guid exerciseId, Guid sectionId, string name, string? description = null, string? Instructions = null, string? Equipment = null, IFormFile? imageFile = default, DifficultyLevel? DifficultyLevel = null, CancellationToken ct = default);
        Task<Result<Deleted>> DeleteCustomExerciseAsync(string userId, Guid exerciseId, CancellationToken ct = default);
    }
}
