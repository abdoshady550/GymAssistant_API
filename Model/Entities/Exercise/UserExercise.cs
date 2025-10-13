using GymAssistant_API.Model.Entities.User;
using GymAssistant_API.Model.Results;

namespace GymAssistant_API.Model.Entities.Exercise
{
    public sealed class UserExercise : Entity
    {
        public string UserId { get; private set; }
        public string Name { get; private set; }
        public string? Description { get; private set; }
        public string? Instructions { get; private set; }
        public string? ImageUrl { get; private set; }
        public string? Equipment { get; private set; }
        public DifficultyLevel? DifficultyLevel { get; private set; }

        public bool IsCustomExercise { get; private set; }
        public Guid SectionId { get; private set; }
        public Section Section { get; private set; } = default!;
        public Guid? SectionGroupId { get; private set; }
        public SectionGroup? SectionGroup { get; private set; } = default!;

        public Guid ClientProfileId { get; set; }
        public ClientProfile User { get; set; } = default!;

        private UserExercise() { }

        private UserExercise(Guid id,
                             string userId,
                             Guid sectionId,
                             string name,
                             string? description = null,
                             string? instructions = null,
                             string? equipment = null,
                             string? imageUrl = null,
                             DifficultyLevel? difficultyLevel = null) : base(id)
        {
            UserId = userId;
            SectionId = sectionId;
            Name = name;
            Description = description;
            Instructions = instructions;
            Equipment = equipment;
            ImageUrl = imageUrl;
            DifficultyLevel = difficultyLevel;

            CreatedAtUtc = DateTimeOffset.UtcNow;
            IsCustomExercise = true;
        }
        public static Result<UserExercise> Create(Guid id,
                                                  string userId,
                                                  Guid sectionId,
                                                  string name,
                                                  string? description = null,
                                                  string? Instructions = null,
                                                  string? Equipment = null,
                                                  string? ImageUrl = null,
                                                  DifficultyLevel? difficultyLevel = null)
        {
            if (userId == null)
            {
                return UserErrors.IdRequired;

            }
            if (sectionId == Guid.Empty)
            {
                return ExerciseErrors.SectionIdRequired;
            }
            if (string.IsNullOrEmpty(name))
            {
                return UserErrors.NameRequired;
            }
            return new UserExercise(id, userId, sectionId, name, description, Instructions, Equipment, ImageUrl, difficultyLevel);
        }
        public Result<Updated> Update(Guid? sectionId,
                                                  string name,
                                                  string? description = null,
                                                  string? instructions = null,
                                                  string? equipment = null,
                                                  string? imageUrl = null,
                                                  DifficultyLevel? difficultyLevel = null)
        {
            if (sectionId.HasValue)
                SectionId = sectionId.Value;
            Name = name;
            Description = description;
            Instructions = instructions;
            Equipment = equipment;
            ImageUrl = imageUrl;
            DifficultyLevel = difficultyLevel;


            return Result.Updated;
        }

    }
}
