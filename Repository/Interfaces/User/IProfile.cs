using GymAssistant_API.Model.Entities.User;
using GymAssistant_API.Model.Results;
using GymAssistant_API.Req_Res.Response;
namespace GymAssistant_API.Repository.Interfaces.User
{
    public interface IProfile
    {
        Task<Result<ClientProfile>> CreateProfileAsync(string userId, string firstName, string lastName, Gender gender, CancellationToken ct = default);
        Task<Result<ProfileResponse>> GetProfileAsync(string userId, int pageSize, int page, CancellationToken ct = default);
        Task<Result<Updated>> UpdateProfileAsync(Guid userId, string firstName, string lastName, Gender gender, DateTime? birthDate, int? heightCm, CancellationToken ct = default);
        Task<Result<BodyMeasurement>> AddBodyMeasurementAsync(string userId, decimal weightKg, decimal? bodyFatPercent = null, decimal? muscleMassKg = null, CancellationToken ct = default);
        Task<Result<Updated>> UpdateBodyMeasurementAsync(Guid Id, decimal weightKg, decimal? bodyFatPercent = null, decimal? muscleMassKg = null, CancellationToken ct = default);
        Task<Result<List<BodyMeasurement>>> GetMeasurementHistoryAsync(string userId, int pageSize, int pageNumber, CancellationToken ct = default);
        Task<Result<object>> GetMeasurementChartsAsync(string userId, int days, CancellationToken ct = default);
        Task<Result<Deleted>> DeleteBodyMeasurementAsync(Guid Id, CancellationToken ct = default);
    }
}
