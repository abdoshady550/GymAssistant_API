using GymAssistant_API.Data;
using GymAssistant_API.Model.Entities.Exercise;
using GymAssistant_API.Model.Results;
using GymAssistant_API.Repository.Interfaces.Exercises;
using Microsoft.EntityFrameworkCore;

namespace GymAssistant_API.Repository.Services.Exercise
{
    public class PersonalRecordService(ILogger<PersonalRecordService> logger, AppDbContext context) : IPersonalRecordService
    {
        private readonly ILogger<PersonalRecordService> _logger = logger;
        private readonly AppDbContext _context = context;

        public async Task CheckAndCreatePersonalRecordsAsync(WorkoutSession session, CancellationToken ct = default)
        {
            foreach (var workoutExercise in session.WorkoutExercises)
            {
                await CheckExerciseRecords(session, workoutExercise, ct);
            }
        }
        private async Task CheckExerciseRecords(WorkoutSession session, WorkoutExercise workoutExercise, CancellationToken ct = default)
        {
            var maxWeightSet = workoutExercise.Sets.OrderByDescending(s => s.WeightKg).FirstOrDefault();
            var maxRepsSet = workoutExercise.Sets.OrderByDescending(s => s.Reps).FirstOrDefault();
            var totalVolume = workoutExercise.Sets.Sum(s => s.WeightKg * s.Reps);

            if (maxWeightSet != null)
            {
                await CheckAndCreateRecord(session.ClientProfileId, session.Id, RecordType.MaxWeight,
                    maxWeightSet.WeightKg, workoutExercise.ExerciseId, workoutExercise.UserExerciseId, maxWeightSet, ct);
            }

            if (maxRepsSet != null)
            {
                await CheckAndCreateRecord(session.ClientProfileId, session.Id, RecordType.MaxReps,
                    maxRepsSet.Reps, workoutExercise.ExerciseId, workoutExercise.UserExerciseId, maxRepsSet, ct);
            }

            if (totalVolume > 0)
            {
                await CheckAndCreateRecord(session.ClientProfileId, session.Id, RecordType.MaxVolume,
                    totalVolume, workoutExercise.ExerciseId, workoutExercise.UserExerciseId, null, ct);
            }
        }

        private async Task CheckAndCreateRecord(Guid clientProfileId,
                                                Guid sessionId,
                                                RecordType recordType,
                                                decimal value,
                                                Guid? exerciseId,
                                                Guid? userExerciseId,
                                                ExerciseSet? set = null, CancellationToken ct = default)
        {

            // التحقق من وجود ExerciseId صالح
            if (exerciseId.HasValue)
            {
                var exerciseExists = await _context.Exercises
                    .AnyAsync(e => e.Id == exerciseId.Value, ct);

                if (!exerciseExists)
                {
                    _logger.LogWarning("Warning: Exercise with ID {exerciseId} not found. Skipping personal record creation.", exerciseId);
                    return;
                }
            }
            else if (!userExerciseId.HasValue)
            {
                _logger.LogWarning("Warning: Both ExerciseId and UserExerciseId are null. Skipping personal record creation.");
                return;
            }
            // Find existing record
            var existingRecord = await _context.PersonalRecords
                .Where(pr => pr.ClientProfileId == clientProfileId &&
                             pr.RecordType == recordType &&
                             pr.ExerciseId == exerciseId &&
                             pr.UserExerciseId == userExerciseId)
                .OrderByDescending(pr => pr.Value)
                .FirstOrDefaultAsync(ct);

            // Check if this is a new record
            if (existingRecord == null || value > existingRecord.Value)
            {
                var recordResult = PersonalRecord.Create(
                    Guid.NewGuid(),
                    clientProfileId,
                    sessionId,
                    recordType,
                    value,
                    exerciseId,
                    userExerciseId);

                if (recordResult.IsSuccess)
                {
                    var record = recordResult.Value;
                    _context.PersonalRecords.Add(record);

                    // Mark the set as a personal record
                    set?.MarkAsPersonalRecord();

                    // Add to profile
                    var profile = await _context.ClientProfiles.FindAsync(clientProfileId, ct);
                    profile?.AddPersonalRecord(record);
                }
                else
                {
                    // Handle error (log it, throw exception, etc.)
                    throw new Exception($"Error creating personal record: {recordResult.Errors.First().Description}");
                }
            }
        }

        public async Task<List<PersonalRecord>> GetPersonalRecordsAsync(string userId, RecordType? recordType = null, CancellationToken ct = default)
        {
            var profile = await _context.ClientProfiles
                .FirstOrDefaultAsync(p => p.AppUserId == userId, ct);

            if (profile == null)
            {
                return new List<PersonalRecord>();
            }

            var query = _context.PersonalRecords
                .Where(pr => pr.ClientProfileId == profile.Id);

            if (recordType.HasValue)
            {
                query = query.Where(pr => pr.RecordType == recordType);
            }

            return await query
                .Include(pr => pr.Exercise)
                .Include(pr => pr.UserExercise)
                .Include(pr => pr.WorkoutSession)
                .OrderByDescending(pr => pr.CreatedAtUtc)
                .ToListAsync(ct);
        }
    }
}
