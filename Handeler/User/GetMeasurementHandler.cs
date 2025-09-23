using GymAssistant_API.Model.Entities.User.Dto;
using GymAssistant_API.Model.Results;
using GymAssistant_API.Repository.Interfaces.User;

namespace GymAssistant_API.Handeler.User
{
    public class GetMeasurementHandler(ILogger<GetMeasurementHandler> logger,
                                   IProfile profile)
    {
        private readonly ILogger<GetMeasurementHandler> logger = logger;
        private readonly IProfile _profile = profile;

        public async Task<Result<List<BodyMeasurementDto>>> GetMeasurementHistoryAsync(string userId,
                                                                                 int pageSize,
                                                                                 int pageNumber,
                                                                                 CancellationToken ct = default)
        {
            logger.LogInformation("Getting measurement history for user {UserId}, page {PageNumber}, page size {PageSize}", userId, pageNumber, pageSize);
            var result = await _profile.GetMeasurementHistoryAsync(userId, pageSize, pageNumber, ct);
            if (result.IsError)
            {
                logger.LogError("Failed to get measurement history for user {UserId}: {Errors}", userId, string.Join(", ", result.Errors.Select(e => e.Description)));
                return result.Errors;
            }
            var dto = result.Value.Select(m => new BodyMeasurementDto
            {
                Id = m.Id,
                WeightKg = m.WeightKg,
                MuscleMassKg = m.MuscleMassKg,
                BodyFatPercent = m.BodyFatPercent,
                CreatedAtUtc = m.CreatedAtUtc
            }).ToList();
            return dto;
        }
        public async Task<Result<object>> GetMeasurementChartsAsync(string userId,
                                                                 int days,
                                                                 CancellationToken ct = default)
        {
            logger.LogInformation("Getting measurement charts for user {UserId} over the last {Days} days", userId, days);
            var result = await _profile.GetMeasurementChartsAsync(userId, days, ct);
            if (result.IsError)
            {
                logger.LogError("Failed to get measurement charts for user {UserId}: {Errors}", userId, string.Join(", ", result.Errors.Select(e => e.Description)));
                return result.Errors;
            }
            return result.Value;
        }
        public async Task<Result<Updated>> UpdateMeasurementAsync(Guid Id,
                                                                      decimal weightKg,
                                                                      decimal? bodyFatPercent = null,
                                                                      decimal? muscleMassKg = null, CancellationToken ct = default)
        {
            logger.LogInformation("Updating measurement for Measurement {MeasurementId}", Id);
            var result = await _profile.UpdateBodyMeasurementAsync(Id, weightKg, bodyFatPercent, muscleMassKg, ct);
            if (result.IsError)
            {
                logger.LogError("Failed to update measurement {MeasurementId}: {Errors}", Id, string.Join(", ", result.Errors.Select(e => e.Description)));
                return result.Errors;
            }
            return Result.Updated;
        }
        public async Task<Result<Deleted>> DeleteMeasurementAsync(Guid Id, CancellationToken ct = default)
        {
            logger.LogInformation("Deleting measurement for Measurement {MeasurementId}", Id);
            var result = await _profile.DeleteBodyMeasurementAsync(Id, ct);
            if (result.IsError)
            {
                logger.LogError("Failed to delete measurement {MeasurementId}: {Errors}", Id, string.Join(", ", result.Errors.Select(e => e.Description)));
                return result.Errors;
            }
            return Result.Deleted;

        }
    }
}
