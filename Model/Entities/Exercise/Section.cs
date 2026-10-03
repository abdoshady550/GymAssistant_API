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
        public string? NameEn { get; private set; }
        public string? NameAr { get; private set; }
        public string? Description { get; private set; }
        public string? DescriptionEn { get; private set; }
        public string? DescriptionAr { get; private set; }
        public string? ImageUrl { get; private set; }

        public ICollection<SectionGroup> SectionGroup => _sectionGroup;

        public ICollection<Exercise> Exercises => _exercises;
        public ICollection<UserExercise> UserExercise => _customExercises;

        private Section() { }

        private Section(Guid id, string name, string? nameAr = null, string? description = null, string? descriptionAr = null, string? imageUrl = null) : base(id)
        {
            Name = name;
            NameEn = name;
            NameAr = nameAr;
            Description = description;
            DescriptionEn = description;
            DescriptionAr = descriptionAr;
            ImageUrl = imageUrl;
            CreatedAtUtc = DateTimeOffset.UtcNow;
        }

        public static Result<Section> Create(Guid id, string name, string? description = null)
        {
            return Create(id, name, null, description, null, null);
        }

        public static Result<Section> Create(Guid id, string nameEn, string? nameAr = null, string? descriptionEn = null, string? descriptionAr = null, string? imageUrl = null)
        {
            if (string.IsNullOrWhiteSpace(nameEn))
            {
                return UserErrors.NameRequired;
            }
            return new Section(id, nameEn, nameAr, descriptionEn, descriptionAr, imageUrl);
        }

        public Result<Updated> Update(string? name = null, string? description = null)
        {
            return Update(name, null, description, null, null);
        }

        public Result<Updated> Update(string? nameEn = null, string? nameAr = null, string? descriptionEn = null, string? descriptionAr = null, string? imageUrl = null)
        {
            if (!string.IsNullOrEmpty(nameEn))
            {
                Name = nameEn;
                NameEn = nameEn;
            }
            if (nameAr != null)
            {
                NameAr = nameAr;
            }
            if (!string.IsNullOrEmpty(descriptionEn))
            {
                Description = descriptionEn;
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

            return Result.Updated;
        }

        public void AddSectionGroup(SectionGroup Group) => _sectionGroup.Add(Group);

        public void AddExercise(Exercise exercise) => _exercises.Add(exercise);

        public void AddUserExercise(UserExercise customExercises) => _customExercises.Add(customExercises);
    }
}
