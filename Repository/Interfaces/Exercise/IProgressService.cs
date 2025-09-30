using GymAssistant_API.Model.Results;
using GymAssistant_API.Req_Res.Response.Progress;

namespace GymAssistant_API.Repository.Interfaces.Exercise
{
    public interface IProgressService
    {
        Task<Result<ExerciseProgressData>> GetExerciseProgressAsync(string userId, Guid exerciseId, int days, CancellationToken ct = default);
        Task<Result<ExerciseProgressData>> GetCustomExerciseProgressAsync(string userId, Guid userExerciseId, int days, CancellationToken ct = default);
        Task<Result<List<SectionProgressData>>> GetSectionProgressAsync(string userId, Guid sectionId, int days, CancellationToken ct = default);
        Task<Result<ProgressOverviewData>> GetProgressOverviewAsync(string userId, int days, CancellationToken ct = default);
        Task<Result<ExerciseChartData>> GetExerciseChartDataAsync(string userId, Guid exerciseId, int days, CancellationToken ct = default);
        Task<Result<VolumeChartData>> GetVolumeChartDataAsync(string userId, int days, Guid? sectionId = null, CancellationToken ct = default);
    }
}
