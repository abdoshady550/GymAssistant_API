using GymAssistant_API.Model.Entities.User;
using GymAssistant_API.Model.Results;

namespace GymAssistant_API.Model.Entities.Exercise
{
    public sealed class WorkoutSession : Entity
    {
        private readonly List<WorkoutExercise> _workoutExercises = new();
        public Guid ClientProfileId { get; private set; }
        public ClientProfile Trainee { get; private set; } = default!;

        public Guid? CreatedByTrainerId { get; private set; }
        public ClientProfile? CreatedByTrainer { get; private set; }

        public DateTime Date { get; private set; }
        public DateTime? StartTime { get; private set; }
        public DateTime? EndTime { get; private set; }
        public bool IsCompleted { get; private set; }
        public string? Notes { get; private set; }


        public IReadOnlyCollection<WorkoutExercise> WorkoutExercises => _workoutExercises.AsReadOnly();

        // Calculated property
        public int? DurationMinutes =>
            StartTime.HasValue && EndTime.HasValue
                ? (int)(EndTime.Value - StartTime.Value).TotalMinutes
                : null;

        private WorkoutSession() { }

        private WorkoutSession(Guid id, Guid clientProfileId, DateTime date, string? notes = null,
                             Guid? createdByTrainerId = null) : base(id)
        {
            ClientProfileId = clientProfileId;
            CreatedByTrainerId = createdByTrainerId;
            Date = date;
            Notes = notes;
            CreatedAtUtc = DateTime.UtcNow;
        }
        public static Result<WorkoutSession> Create(Guid id,
                                                    Guid clientProfileId,
                                                    DateTime date,
                                                    string? notes = null,
                                                    Guid? createdByTrainerId = null)
        {
            if (clientProfileId == Guid.Empty)
            {
                return ExerciseErrors.ClientProfileIdRequired;
            }
            if (date == default)
            {
                return ExerciseErrors.DateRequired;
            }
            if (createdByTrainerId != null && createdByTrainerId == Guid.Empty)
            {
                return ExerciseErrors.CreatedByTrainerIdInvalid;
            }
            return new WorkoutSession(id, clientProfileId, date, notes, createdByTrainerId);
        }

        public void StartWorkout(DateTime startTime) => StartTime = startTime;
        public void CompleteWorkout(DateTime endTime, string? notes = null)
        {
            EndTime = endTime;
            IsCompleted = true;
            if (notes != null) Notes = notes;
        }

        public void AddWorkoutExercise(WorkoutExercise workoutExercise) => _workoutExercises.Add(workoutExercise);
    }
}

