using GymAssistant_API.Model.Entities.Exercise;

namespace GymAssistant_API.Repository.Interfaces.Exercises
{
    public interface IPersonalRecordService
    {
        Task CheckAndCreatePersonalRecordsAsync(WorkoutSession session, CancellationToken ct = default);
        Task<List<PersonalRecord>> GetPersonalRecordsAsync(string userId, RecordType? recordType = null, CancellationToken ct = default);
    }

}
