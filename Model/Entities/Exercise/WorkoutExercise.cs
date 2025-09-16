using GymAssistant_API.Model.Entities.User;
using GymAssistant_API.Model.Results;

namespace GymAssistant_API.Model.Entities.Exercise
{
    public sealed class WorkoutExercise : Entity
    {
        private readonly List<ExerciseSet> _sets = new();


        public Guid WorkoutSessionId { get; private set; }
        public Guid? ExerciseId { get; private set; }
        public Guid? UserExerciseId { get; private set; }

        public Guid ClientProfileId { get; private set; }
        public ClientProfile User { get; private set; } = default!;
        public IReadOnlyCollection<ExerciseSet> Sets => _sets.AsReadOnly();

        private WorkoutExercise() { }

        private WorkoutExercise(Guid id, Guid workoutSessionId, Guid? exerciseId = null, Guid? userExerciseId = null) : base(id)
        {
            WorkoutSessionId = workoutSessionId;
            ExerciseId = exerciseId;
            UserExerciseId = userExerciseId;
            CreatedAtUtc = DateTime.UtcNow;
        }
        public static Result<WorkoutExercise> Create(Guid id, Guid workoutSessionId, Guid? exerciseId = null, Guid? userExerciseId = null)
        {
            if (workoutSessionId == Guid.Empty)
            {
                return ExerciseErrors.WorkoutExerciseIdRequired;
            }
            return new WorkoutExercise(id, workoutSessionId, exerciseId, userExerciseId);
        }
        public void AddSet(ExerciseSet set) => _sets.Add(set);
    }
}