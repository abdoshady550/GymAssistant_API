using GymAssistant_API.Model.Entities.User;
using GymAssistant_API.Model.Results;

namespace GymAssistant_API.Model.Entities.Exercise
{
    public sealed class PersonalRecord : Entity
    {
        public Guid ClientProfileId { get; private set; }
        public ClientProfile User { get; private set; } = default!;

        public Guid? ExerciseId { get; private set; }
        public Exercise? Exercise { get; private set; }

        public Guid? UserExerciseId { get; private set; }
        public UserExercise? UserExercise { get; private set; }

        public RecordType RecordType { get; private set; }
        public decimal Value { get; private set; }// Record Value

        public Guid WorkoutSessionId { get; private set; }
        public WorkoutSession WorkoutSession { get; private set; } = default!;

        private PersonalRecord() { }

        private PersonalRecord(Guid id,
                               Guid clientProfileId,
                               Guid workoutSessionId,
                               RecordType recordType,
                               decimal value,
                               Guid? exerciseId = null,
                               Guid? userExerciseId = null) : base(id)
        {
            ClientProfileId = clientProfileId;
            WorkoutSessionId = workoutSessionId;
            RecordType = recordType;
            Value = value;
            ExerciseId = exerciseId;
            UserExerciseId = userExerciseId;
            CreatedAtUtc = DateTime.UtcNow;
        }
        public static Result<PersonalRecord> Create(Guid id, Guid clientProfileId, Guid workoutSessionId,
                                                    RecordType recordType, decimal value,
                                                    Guid? exerciseId = null, Guid? userExerciseId = null)
        {
            if (clientProfileId == Guid.Empty)
            {
                return ExerciseErrors.ClientProfileIdRequired;
            }
            if (value <= 0)
            {
                return ExerciseErrors.PersonalRecordValueISInvalid;
            }
            if (exerciseId == null && userExerciseId == null)
            {
                return ExerciseErrors.PersonalRecordExerciseIsRequired;
            }
            if (exerciseId != null && userExerciseId != null)
            {
                return ExerciseErrors.PersonalRecordExerciseIsConflict;
            }
            return new PersonalRecord(id, clientProfileId, workoutSessionId, recordType, value, exerciseId, userExerciseId);
        }
    }

    public enum RecordType
    {
        MaxWeight = 1,
        MaxReps = 2,
        MaxVolume = 3
    }

}
