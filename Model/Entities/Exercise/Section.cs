using GymAssistant_API.Model.Entities.User;
using GymAssistant_API.Model.Results;

namespace GymAssistant_API.Model.Entities.Exercise
{
    public sealed class Section : Entity
    {
        private readonly List<Exercise> _exercises = new();
        private readonly List<UserExercise> _customExercises = new();
        private readonly List<SectionGroup> _sectionGroup = new();


        public string Name { get; private set; }
        public string? Description { get; private set; }

        public ICollection<SectionGroup> SectionGroup => _sectionGroup;

        public ICollection<Exercise> Exercises => _exercises;
        public ICollection<UserExercise> UserExercise => _customExercises;


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
        public Result<Updated> Update(string? name = null, string? description = null)
        {
            if (!string.IsNullOrEmpty(name))
            {
                Name = name;
            }
            if (!string.IsNullOrEmpty(description))
            {
                Description = description;
            }

            return Result.Updated;
        }

        public void AddSectionGroup(SectionGroup Group) => _sectionGroup.Add(Group);

        public void AddExercise(Exercise exercise) => _exercises.Add(exercise);

        public void AddUserExercise(UserExercise customExercises) => _customExercises.Add(customExercises);

    }
}
