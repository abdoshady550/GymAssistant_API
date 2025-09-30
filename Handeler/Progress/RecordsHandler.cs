using GymAssistant_API.Model.Entities.Exercise;
using GymAssistant_API.Model.Results;
using GymAssistant_API.Repository.Interfaces.Exercise;
using GymAssistant_API.Repository.Services.Progress;
using GymAssistant_API.Req_Res.Response.Records;
using Microsoft.AspNetCore.Mvc;

namespace GymAssistant_API.Handeler.Progress
{
    public class RecordsHandler(ILogger<RecordsHandler> logger,
                                IRecordsService records)
    {
        private readonly ILogger<RecordsHandler> _logger = logger;
        private readonly IRecordsService _recordsService = records;
        public async Task<Result<List<PersonalRecordResponse>>> GetPersonalRecords(string userId, RecordType? recordType = null, CancellationToken ct = default)
        {
            var result = await _recordsService.GetPersonalRecordsAsync(userId, recordType, ct);
            if (result.IsError)
            {
                _logger.LogError("Error getting personal records for user {UserId}: {Errors}", userId, result.TopError.Description);
                return result.Errors;
            }
            return result.Value;
        }
        public async Task<Result<List<PersonalRecordResponse>>> GetExerciseRecords(string userId, Guid exerciseId, CancellationToken ct = default)
        {
            var result = await _recordsService.GetExerciseRecordsAsync(userId, exerciseId, ct);
            if (result.IsError)
            {
                _logger.LogError("Error getting exercise records for user {UserId} and exercise {ExerciseId}: {Errors}", userId, exerciseId, result.TopError.Description);
                return result.Errors;
            }
            return result.Value;
        }
        public async Task<Result<List<PersonalRecordResponse>>> GetCustomExerciseRecords(string userId, Guid userExerciseId, CancellationToken ct = default)
        {
            var result = await _recordsService.GetCustomExerciseRecordsAsync(userId, userExerciseId, ct);
            if (result.IsError)
            {
                _logger.LogError("Error getting custom exercise records for user {UserId} and user exercise {UserExerciseId}: {Errors}", userId, userExerciseId, result.TopError.Description);
                return result.Errors;
            }
            return result.Value;
        }
        public async Task<Result<List<PersonalRecordResponse>>> GetRecentRecords(string userId, int count, CancellationToken ct = default)
        {
            var result = await _recordsService.GetRecentRecordsAsync(userId, count, ct);
            if (result.IsError)
            {
                _logger.LogError("Error getting recent records for user {UserId}: {Errors}", userId, result.TopError.Description);
                return result.Errors;
            }
            return result.Value;
        }
        public async Task<Result<AchievementsData>> GetAchievements(string userId, DateTime? fromDate = null, DateTime? toDate = null, CancellationToken ct = default)
        {
            var result = await _recordsService.GetAchievementsAsync(userId, fromDate, toDate, ct);
            if (result.IsError)
            {
                _logger.LogError("Error getting achievements for user {UserId}: {Errors}", userId, result.TopError.Description);
                return result.Errors;
            }
            return result.Value;

        }
    }
}
