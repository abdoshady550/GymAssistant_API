using GymAssistant_API.Model.Results;
using GymAssistant_API.Repository.Interfaces.Exercise;
using GymAssistant_API.Repository.Services.Progress;
using GymAssistant_API.Req_Res.Response.Progress;
using Microsoft.AspNetCore.Mvc;

namespace GymAssistant_API.Handeler.Progress
{
    public sealed class ProgressHandler(ILogger<ProgressHandler> logger, IProgressService progress)
    {
        private readonly ILogger<ProgressHandler> _logger = logger;
        private readonly IProgressService _progress = progress;

        public async Task<Result<ExerciseProgressData>> GetExerciseProgress(string userId, Guid exerciseId, int days = 30, CancellationToken ct = default)
        {
            var result = await _progress.GetExerciseProgressAsync(userId, exerciseId, days, ct);
            if (result.IsError)
            {
                _logger.LogError("Error getting exercise progress for user {UserId} and exercise {ExerciseId}: {ErrorMessage}", userId, exerciseId, result.TopError.Description);
                return result.Errors;
            }
            return result.Value;
        }
        public async Task<Result<ExerciseProgressData>> GetCustomExerciseProgress(string userId, Guid userExerciseId, int days = 30, CancellationToken ct = default)
        {
            var result = await _progress.GetCustomExerciseProgressAsync(userId, userExerciseId, days, ct);
            if (result.IsError)
            {
                _logger.LogError("Error getting custom exercise progress for user {UserId} and user exercise {UserExerciseId}: {ErrorMessage}", userId, userExerciseId, result.TopError.Description);
                return result.Errors;
            }
            _logger.LogInformation("Successfully retrieved custom exercise progress for user {UserId} and user exercise {UserExerciseId}", userId, userExerciseId);
            return result.Value;


        }
        public async Task<Result<List<SectionProgressData>>> GetSectionProgress(string userId, Guid sectionId, int days = 30, CancellationToken ct = default)
        {
            var result = await _progress.GetSectionProgressAsync(userId, sectionId, days, ct);
            if (result.IsError)
            {
                _logger.LogError("Error getting section progress for user {UserId} and section {SectionId}: {ErrorMessage}", userId, sectionId, result.TopError.Description);
                return result.Errors;
            }
            _logger.LogInformation("Successfully retrieved section progress for user {UserId} and section {SectionId}", userId, sectionId);
            return result.Value;

        }
        public async Task<Result<ProgressOverviewData>> GetProgressOverview(string userId, int days = 7, CancellationToken ct = default)
        {
            var result = await _progress.GetProgressOverviewAsync(userId, days, ct);
            if (result.IsError)
            {
                _logger.LogError("Error getting progress overview for user {UserId}: {ErrorMessage}", userId, result.TopError.Description);
                return result.Errors;
            }
            _logger.LogInformation("Successfully retrieved progress overview for user {UserId}", userId);
            return result.Value;

        }
        public async Task<Result<ExerciseChartData>> GetExerciseChartData(string userId, Guid exerciseId, int days = 30, CancellationToken ct = default)
        {
            var result = await _progress.GetExerciseChartDataAsync(userId, exerciseId, days, ct);
            if (result.IsError)
            {
                _logger.LogError("Error getting exercise chart data for user {UserId} and exercise {ExerciseId}: {ErrorMessage}", userId, exerciseId, result.TopError.Description);
                return result.Errors;
            }
            _logger.LogInformation("Successfully retrieved exercise chart data for user {UserId} and exercise {ExerciseId}", userId, exerciseId);
            return result.Value;
        }
        public async Task<Result<VolumeChartData>> GetVolumeChartData(string userId, int days = 30, Guid? sectionId = null, CancellationToken ct = default)
        {
            var result = await _progress.GetVolumeChartDataAsync(userId, days, sectionId, ct);
            if (result.IsError)
            {
                _logger.LogError("Error getting volume chart data for user {UserId} and section {SectionId}: {ErrorMessage}", userId, sectionId, result.TopError.Description);
                return result.Errors;
            }
            _logger.LogInformation("Successfully retrieved volume chart data for user {UserId} and section {SectionId}", userId, sectionId);

            return result.Value;
        }





    }
}
