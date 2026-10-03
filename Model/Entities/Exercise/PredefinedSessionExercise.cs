using GymAssistant_API.Model.Results;

namespace GymAssistant_API.Model.Entities.Exercise
{
    public sealed class PredefinedSessionExercise : Entity
    {
        public Guid PredefinedWorkoutSessionId { get; private set; }
        public PredefinedWorkoutSession PredefinedWorkoutSession { get; private set; } = default!;

        public Guid ExerciseId { get; private set; }
        public Exercise Exercise { get; private set; } = default!;

        public int Order { get; private set; }
        public int DefaultSets { get; private set; }
        public int DefaultReps { get; private set; }
        public int? DefaultRestTimeSeconds { get; private set; }

#pragma warning disable CS8618
        private PredefinedSessionExercise() { }
#pragma warning restore CS8618

        private PredefinedSessionExercise(Guid id, Guid sessionId, Guid exerciseId, int order,
                                         int defaultSets, int defaultReps, int? defaultRestTimeSeconds) : base(id)
        {
            PredefinedWorkoutSessionId = sessionId;
            ExerciseId = exerciseId;
            Order = order;
            DefaultSets = defaultSets;
            DefaultReps = defaultReps;
            DefaultRestTimeSeconds = defaultRestTimeSeconds;
            CreatedAtUtc = DateTimeOffset.UtcNow;
        }

        public static Result<PredefinedSessionExercise> Create(Guid id, Guid sessionId, Guid exerciseId,
                                                              int order = 1, int defaultSets = 3, int defaultReps = 10,
                                                              int? defaultRestTimeSeconds = 60)
        {
            if (sessionId == Guid.Empty)
            {
                return ExerciseErrors.SessionNotFound;
            }
            if (exerciseId == Guid.Empty)
            {
                return ExerciseErrors.ExerciseIdRequired;
            }
            if (defaultSets <= 0)
            {
                return ExerciseErrors.DefaultSetsInvalid;
            }
            if (defaultReps <= 0)
            {
                return ExerciseErrors.DefaultRepsInvalid;
            }

            return new PredefinedSessionExercise(id, sessionId, exerciseId, order, defaultSets, defaultReps, defaultRestTimeSeconds);
        }
    }
}
