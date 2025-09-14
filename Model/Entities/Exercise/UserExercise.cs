using GymAssistant_API.Model.Entities.User;
using GymAssistant_API.Model.Results;

namespace GymAssistant_API.Model.Entities.Exercise
{
    public class UserExercise : Entity
    {
        public string UserId { get; private set; }
        public string Name { get; private set; }
        public string? Description { get; private set; }

        public Guid ClientProfileId { get; private set; }
        public ClientProfile User { get; private set; } = default!;

        private UserExercise() { }

        private UserExercise(Guid id, string userId, string name, string? description = null) : base(id)
        {
            UserId = userId;
            Name = name;
            Description = description;
            CreatedAtUtc = DateTimeOffset.UtcNow;
        }
        public static Result<UserExercise> Create(Guid id, string userId, string name, string? description = null)
        {
            if (userId == null)
            {
                return UserErrors.IdRequired;

            }
            if (string.IsNullOrEmpty(name))
            {
                return UserErrors.NameRequired;
            }
            return new UserExercise(id, userId, name, description);
        }
    }
}
