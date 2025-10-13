using GymAssistant_API.Model.Entities.User;
using GymAssistant_API.Model.Results;
using GymAssistant_API.Repository.Interfaces.ExerciseExercises;
using static System.Collections.Specialized.BitVector32;

namespace GymAssistant_API.Model.Entities.Exercise
{
    public sealed class SectionGroup : Entity
    {


        private readonly List<UserExercise> _CustomExercises = new();

        private readonly List<Exercise> _exercises = new();

        public string Name { get; private set; }
        public string? Description { get; private set; }

        public Guid SectionId { get; private set; }
        public Section Section { get; private set; } = default!;
        public Guid ClientProfileId { get; set; }
        public ClientProfile User { get; set; } = default!;
        public ICollection<Exercise> Exercises => _exercises;
        public ICollection<UserExercise> UserExercise => _CustomExercises;

        private SectionGroup() { }
        private SectionGroup(Guid id, Guid sectionId, string name, string? description = null) : base(id)
        {
            SectionId = sectionId;
            Name = name;
            Description = description;
            CreatedAtUtc = DateTimeOffset.UtcNow;
        }
        public static Result<SectionGroup> Create(Guid id, Guid sectionId, string name, string? description = null)
        {
            if (sectionId == Guid.Empty)
            {
                return ExerciseErrors.SectionIdRequired;
            }
            if (string.IsNullOrEmpty(name))
            {
                return UserErrors.NameRequired;
            }
            return new SectionGroup(id, sectionId, name, description);
        }
        public Result<Updated> Update(string? name = null, string? description = null)
        {

            if (!string.IsNullOrEmpty(name))
                Name = name;

            if (!string.IsNullOrEmpty(description))
                Description = description;

            return Result.Updated;
        }

        public void AddExercise(Exercise exercise) => _exercises.Add(exercise);
        public void AddUserExercise(UserExercise customExercises) => _CustomExercises.Add(customExercises);
        public void RemoveExercise(Exercise exercise) => _exercises.Remove(exercise);
        public void RemoveUserExercise(UserExercise customExercises) => _CustomExercises.Remove(customExercises);


    }
}
