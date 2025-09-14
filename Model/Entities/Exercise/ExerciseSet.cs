using GymAssistant_API.Model.Results;

namespace GymAssistant_API.Model.Entities.Exercise
{
    public class ExerciseSet : Entity
    {

        public Guid WorkoutExerciseId { get; private set; }
        public WorkoutExercise WorkoutExercise { get; private set; } = default!;
        public int SetNumber { get; private set; }
        public int Reps { get; private set; }

        public decimal WeightKg { get; private set; }
        public int? RestTimeSeconds { get; private set; }
        public bool IsCompleted { get; private set; } = true;
        public bool IsPersonalRecord { get; private set; }
        public string? Notes { get; private set; }


        private ExerciseSet() { }

        private ExerciseSet(Guid id, Guid workoutExerciseId, int setNumber, int reps, decimal weightKg,
                          int? restTimeSeconds = null, string? notes = null) : base(id)
        {
            WorkoutExerciseId = workoutExerciseId;
            SetNumber = setNumber;
            Reps = reps;
            WeightKg = weightKg;
            RestTimeSeconds = restTimeSeconds;
            Notes = notes;
            CreatedAtUtc = DateTime.UtcNow;
        }
        public static Result<ExerciseSet> Create(Guid id,
                                                 Guid workoutExerciseId,
                                                 int setNumber,
                                                 int reps,
                                                 decimal weightKg,
                                                 int? restTimeSeconds = null,
                                                 string? notes = null)
        {
            if (workoutExerciseId == Guid.Empty)
            {
                return ExerciseErrors.WorkoutExerciseIdRequired;
            }
            if (setNumber <= 0)
            {
                return ExerciseErrors.SetNumberInvalid;
            }
            if (reps <= 0)
            {
                return ExerciseErrors.RepsInvalid;
            }
            if (weightKg < 0)
            {
                return ExerciseErrors.WeightKgInvalid;
            }
            if (restTimeSeconds != null && restTimeSeconds < 0)
            {
                return ExerciseErrors.RestTimeSecondsInvalid;
            }
            return new ExerciseSet(id, workoutExerciseId, setNumber, reps, weightKg, restTimeSeconds, notes);
        }

        public void MarkAsPersonalRecord() => IsPersonalRecord = true;
        public void MarkAsIncomplete() => IsCompleted = false;
    }
}