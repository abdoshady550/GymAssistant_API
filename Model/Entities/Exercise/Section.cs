using GymAssistant_API.Model.Entities.User;
using GymAssistant_API.Model.Results;

namespace GymAssistant_API.Model.Entities.Exercise
{
    public sealed class Section : Entity
    {
        private readonly List<Exercise> _exercises = new();

        public string Name { get; private set; }
        public string? Description { get; private set; }

        public IReadOnlyCollection<Exercise> Exercises => _exercises.AsReadOnly();

        private Section() { }

        private Section(Guid id, string name, string? description = null) : base(id)
        {
            Name = name;
            Description = description;
            CreatedAtUtc = DateTimeOffset.UtcNow;
        }
        public static Result<Section> Create(Guid id, string name, string? description = null)
        {
            if (string.IsNullOrEmpty(name))
            {
                return UserErrors.NameRequired;

            }
            return new Section(id, name, description);

        }

        public void AddExercise(Exercise exercise) => _exercises.Add(exercise);
    }
}
