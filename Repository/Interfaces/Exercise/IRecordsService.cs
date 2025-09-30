using GymAssistant_API.Model.Entities.Exercise;
using GymAssistant_API.Model.Results;
using GymAssistant_API.Req_Res.Response.Records;

namespace GymAssistant_API.Repository.Interfaces.Exercise
{
    public interface IRecordsService
    {
        Task<Result<List<PersonalRecordResponse>>> GetPersonalRecordsAsync(string userId, RecordType? recordType = null, CancellationToken ct = default);
        Task<Result<List<PersonalRecordResponse>>> GetExerciseRecordsAsync(string userId, Guid exerciseId, CancellationToken ct = default);
        Task<Result<List<PersonalRecordResponse>>> GetCustomExerciseRecordsAsync(string userId, Guid userExerciseId, CancellationToken ct = default);
        Task<Result<List<PersonalRecordResponse>>> GetRecentRecordsAsync(string userId, int count, CancellationToken ct = default);
        Task<Result<AchievementsData>> GetAchievementsAsync(string userId, DateTime? fromDate = null, DateTime? toDate = null, CancellationToken ct = default);
    }
}
