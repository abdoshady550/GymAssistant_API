using GymAssistant_API.Model.Results;

namespace GymAssistant_API.Model.Entities.Exercise
{
    public sealed class PredefinedWorkday : Entity
    {
        private readonly List<PredefinedWorkoutSession> _sessions = new();

        public string NameEn { get; private set; }
        public string? NameAr { get; private set; }
        public string? DescriptionEn { get; private set; }
        public string? DescriptionAr { get; private set; }
        public string? ImageUrl { get; private set; }
        public int? DayNumber { get; private set; }
        public bool IsActive { get; private set; }

        public IReadOnlyCollection<PredefinedWorkoutSession> Sessions => _sessions.AsReadOnly();

#pragma warning disable CS8618
        private PredefinedWorkday() { }
#pragma warning restore CS8618

        private PredefinedWorkday(Guid id, string nameEn, string? nameAr, string? descriptionEn,
                                  string? descriptionAr, string? imageUrl, int? dayNumber, bool isActive) : base(id)
        {
            NameEn = nameEn;
            NameAr = nameAr;
            DescriptionEn = descriptionEn;
            DescriptionAr = descriptionAr;
            ImageUrl = imageUrl;
            DayNumber = dayNumber;
            IsActive = isActive;
            CreatedAtUtc = DateTimeOffset.UtcNow;
        }

        public static Result<PredefinedWorkday> Create(Guid id, string nameEn, string? nameAr = null,
                                                      string? descriptionEn = null, string? descriptionAr = null,
                                                      string? imageUrl = null, int? dayNumber = null, bool isActive = true)
        {
            if (string.IsNullOrWhiteSpace(nameEn))
            {
                return ExerciseErrors.NameRequired;
            }

            return new PredefinedWorkday(id, nameEn, nameAr, descriptionEn, descriptionAr, imageUrl, dayNumber, isActive);
        }

        public Result<Updated> Update(string? nameEn = null, string? nameAr = null,
                                      string? descriptionEn = null, string? descriptionAr = null,
                                      string? imageUrl = null, int? dayNumber = null, bool? isActive = null)
        {
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
            if (dayNumber.HasValue)
            {
                DayNumber = dayNumber.Value;
            }
            if (isActive.HasValue)
            {
                IsActive = isActive.Value;
            }

            return Result.Updated;
        }

        public void AddSession(PredefinedWorkoutSession session) => _sessions.Add(session);
    }
}
