using GymAssistant_API.Model.Results;

namespace GymAssistant_API.Model.Entities.Exercise
{
    public sealed class PredefinedWorkoutSession : Entity
    {
        private readonly List<PredefinedSessionExercise> _exercises = new();

        public Guid? PredefinedWorkdayId { get; private set; }
        public PredefinedWorkday? PredefinedWorkday { get; private set; }

        public string NameEn { get; private set; }
        public string? NameAr { get; private set; }
        public string? DescriptionEn { get; private set; }
        public string? DescriptionAr { get; private set; }
        public string? ImageUrl { get; private set; }
        public DifficultyLevel? DifficultyLevel { get; private set; }
        public int? EstimatedDurationMinutes { get; private set; }
        public bool IsActive { get; private set; }

        public IReadOnlyCollection<PredefinedSessionExercise> Exercises => _exercises.AsReadOnly();

#pragma warning disable CS8618
        private PredefinedWorkoutSession() { }
#pragma warning restore CS8618

        private PredefinedWorkoutSession(Guid id, Guid? workdayId, string nameEn, string? nameAr,
                                         string? descriptionEn, string? descriptionAr, string? imageUrl,
                                         DifficultyLevel? difficultyLevel, int? estimatedDurationMinutes,
                                         bool isActive) : base(id)
        {
            PredefinedWorkdayId = workdayId;
            NameEn = nameEn;
            NameAr = nameAr;
            DescriptionEn = descriptionEn;
            DescriptionAr = descriptionAr;
            ImageUrl = imageUrl;
            DifficultyLevel = difficultyLevel;
            EstimatedDurationMinutes = estimatedDurationMinutes;
            IsActive = isActive;
            CreatedAtUtc = DateTimeOffset.UtcNow;
        }

        public static Result<PredefinedWorkoutSession> Create(Guid id, Guid? workdayId, string nameEn,
                                                             string? nameAr = null, string? descriptionEn = null,
                                                             string? descriptionAr = null, string? imageUrl = null,
                                                             DifficultyLevel? difficultyLevel = null,
                                                             int? estimatedDurationMinutes = null,
                                                             bool isActive = true)
        {
            if (string.IsNullOrWhiteSpace(nameEn))
            {
                return ExerciseErrors.NameRequired;
            }

            return new PredefinedWorkoutSession(id, workdayId, nameEn, nameAr, descriptionEn, descriptionAr,
                                               imageUrl, difficultyLevel, estimatedDurationMinutes, isActive);
        }

        public Result<Updated> Update(Guid? workdayId = null, string? nameEn = null, string? nameAr = null,
                                      string? descriptionEn = null, string? descriptionAr = null,
                                      string? imageUrl = null, DifficultyLevel? difficultyLevel = null,
                                      int? estimatedDurationMinutes = null, bool? isActive = null)
        {
            if (workdayId.HasValue)
            {
                PredefinedWorkdayId = workdayId.Value == Guid.Empty ? null : workdayId.Value;
            }
            if (!string.IsNullOrEmpty(nameEn))
            {
                NameEn = nameEn;
            }
            if (nameAr != null)
            {
                NameAr = nameAr;
            }
            if (descriptionEn != null)
            {
                DescriptionEn = descriptionEn;
            }
            if (descriptionAr != null)
            {
                DescriptionAr = descriptionAr;
            }
            if (imageUrl != null)
            {
                ImageUrl = imageUrl;
            }
            if (difficultyLevel.HasValue)
            {
                DifficultyLevel = difficultyLevel.Value;
            }
            if (estimatedDurationMinutes.HasValue)
            {
                EstimatedDurationMinutes = estimatedDurationMinutes.Value;
            }
            if (isActive.HasValue)
            {
                IsActive = isActive.Value;
            }

            return Result.Updated;
        }

        public void AddExercise(PredefinedSessionExercise exercise) => _exercises.Add(exercise);
        public void ClearExercises() => _exercises.Clear();
    }
}
