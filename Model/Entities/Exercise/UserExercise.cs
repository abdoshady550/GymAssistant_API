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

        public Guid ClientProfileId { get; set; }
        public ClientProfile User { get; set; } = default!;

        private UserExercise() { }

        private UserExercise(Guid id, string userId, string name, string? description = null, string? Instructions = null, string? Equipment = null, string? ImageUrl = null) : base(id)
        {
            UserId = userId;
            Name = name;
            Description = description;
            CreatedAtUtc = DateTimeOffset.UtcNow;
        }
        public static Result<UserExercise> Create(Guid id,
                                                  string userId,
                                                  string name,
                                                  string? description = null,
                                                  string? Instructions = null,
                                                  string? Equipment = null,
                                                  string? ImageUrl = null)
        {
            if (userId == null)
            {
                return UserErrors.IdRequired;

            }
            if (string.IsNullOrEmpty(name))
            {
                return UserErrors.NameRequired;
            }
            return new UserExercise(id, userId, name, description, Instructions, Equipment, ImageUrl);
        }
        public Result<Updated> Update(
                                                  string name,
                                                  string? description = null,
                                                  string? Instructions = null,
                                                  string? Equipment = null,
                                                  string? ImageUrl = null)
        {
            Name = name;
            Description = description;
            Instructions = Instructions;
            Equipment = Equipment;
            ImageUrl = ImageUrl;

            return Result.Updated;
        }

    }
}
