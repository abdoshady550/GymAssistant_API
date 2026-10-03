using GymAssistant_API.Model.Identity;
using GymAssistant_API.Model.Results;

namespace GymAssistant_API.Model.Entities.Exercise
{
    public sealed class Exercise : Entity
    {
        public Guid SectionId { get; private set; }
        public Section Section { get; private set; } = default!;
        public Guid? SectionGroupId { get; private set; }
        public SectionGroup? SectionGroup { get; private set; } = default!;

        public string Name { get; private set; }
        public string? NameEn { get; private set; }
        public string? NameAr { get; private set; }
        public string? Description { get; private set; }
        public string? DescriptionEn { get; private set; }
        public string? DescriptionAr { get; private set; }
        public string? Instructions { get; private set; }
        public string? InstructionsEn { get; private set; }
        public string? InstructionsAr { get; private set; }
        public string? ImageUrl { get; private set; }
        public string? Equipment { get; private set; }
        public string? EquipmentEn { get; private set; }
        public string? EquipmentAr { get; private set; }
        public DifficultyLevel? DifficultyLevel { get; private set; }
        public int? DefaultSets { get; private set; }
        public int? DefaultReps { get; private set; }
        public bool IsCustomExercise { get; private set; }

#pragma warning disable CS8618
        private Exercise() { }
#pragma warning restore CS8618

        private Exercise(Guid id, Guid sectionId, string name, string? description = null,
                       string? instructions = null, string? imageUrl = null,
                       string? equipment = null, DifficultyLevel? difficultyLevel = null,
                       int? defaultSets = null, int? defaultReps = null,
                       string? nameAr = null, string? descriptionAr = null,
                       string? instructionsAr = null, string? equipmentAr = null) : base(id)
        {
            SectionId = sectionId;
            Name = name;
            NameEn = name;
            NameAr = nameAr;
            Description = description;
            DescriptionEn = description;
            DescriptionAr = descriptionAr;
            Instructions = instructions;
            InstructionsEn = instructions;
            InstructionsAr = instructionsAr;
            ImageUrl = imageUrl;
            Equipment = equipment;
            EquipmentEn = equipment;
            EquipmentAr = equipmentAr;
            DifficultyLevel = difficultyLevel;
            DefaultSets = defaultSets;
            DefaultReps = defaultReps;
            IsCustomExercise = false;
            CreatedAtUtc = DateTime.UtcNow;
        }

        public static Result<Exercise> Create(Guid id, Guid sectionId, string name, string? description = null,
                                            string? instructions = null, string? imageUrl = null,
                                            string? equipment = null, DifficultyLevel? difficultyLevel = null,
                                            int? defaultSets = null, int? defaultReps = null)
        {
            return Create(id, sectionId, name, null, description, null, instructions, null,
                          imageUrl, equipment, null, difficultyLevel, defaultSets, defaultReps);
        }

        public static Result<Exercise> Create(Guid id, Guid sectionId, string nameEn, string? nameAr = null,
                                            string? descriptionEn = null, string? descriptionAr = null,
                                            string? instructionsEn = null, string? instructionsAr = null,
                                            string? imageUrl = null, string? equipmentEn = null, string? equipmentAr = null,
                                            DifficultyLevel? difficultyLevel = null,
                                            int? defaultSets = null, int? defaultReps = null)
        {
            if (sectionId == Guid.Empty)
            {
                return ExerciseErrors.SectionIdRequired;
            }
            if (string.IsNullOrWhiteSpace(nameEn))
            {
                return ExerciseErrors.NameRequired;
            }
            if (defaultSets != null && defaultSets <= 0)
            {
                return ExerciseErrors.DefaultSetsInvalid;
            }
            if (defaultReps != null && defaultReps <= 0)
            {
                return ExerciseErrors.DefaultRepsInvalid;
            }

            var exercise = new Exercise(id, sectionId, nameEn, descriptionEn, instructionsEn, imageUrl,
                                        equipmentEn, difficultyLevel, defaultSets, defaultReps,
                                        nameAr, descriptionAr, instructionsAr, equipmentAr);
            return exercise;
        }

        public Result<Updated> Update(Guid? sectionId,
                                      string? name,
                                      string? description,
                                      string? instructions,
                                      string? imageUrl,
                                      string? equipment,
                                      DifficultyLevel? difficultyLevel,
                                      int? defaultSets,
                                      int? defaultReps)
        {
            return Update(sectionId, name, null, description, null, instructions, null,
                          imageUrl, equipment, null, difficultyLevel, defaultSets, defaultReps);
        }

        public Result<Updated> Update(Guid? sectionId,
                                      string? nameEn,
                                      string? nameAr,
                                      string? descriptionEn,
                                      string? descriptionAr,
                                      string? instructionsEn,
                                      string? instructionsAr,
                                      string? imageUrl,
                                      string? equipmentEn,
                                      string? equipmentAr,
                                      DifficultyLevel? difficultyLevel,
                                      int? defaultSets,
                                      int? defaultReps)
        {
            if (defaultSets != null && defaultSets <= 0)
            {
                return ExerciseErrors.DefaultSetsInvalid;
            }
            if (defaultReps != null && defaultReps <= 0)
            {
                return ExerciseErrors.DefaultRepsInvalid;
            }
            if (sectionId.HasValue)
                SectionId = sectionId.Value;
            if (!string.IsNullOrEmpty(nameEn))
            {
                Name = nameEn;
                NameEn = nameEn;
            }
            if (nameAr != null)
            {
                NameAr = nameAr;
            }
            if (descriptionEn != null)
            {
                Description = descriptionEn;
                DescriptionEn = descriptionEn;
            }
            if (descriptionAr != null)
            {
                DescriptionAr = descriptionAr;
            }
            if (instructionsEn != null)
            {
                Instructions = instructionsEn;
                InstructionsEn = instructionsEn;
            }
            if (instructionsAr != null)
            {
                InstructionsAr = instructionsAr;
            }
            if (equipmentEn != null)
            {
                Equipment = equipmentEn;
                EquipmentEn = equipmentEn;
            }
            if (equipmentAr != null)
            {
                EquipmentAr = equipmentAr;
            }
            if (!string.IsNullOrEmpty(imageUrl))
            {
                ImageUrl = imageUrl;
            }
            if (difficultyLevel.HasValue)
            {
                DifficultyLevel = difficultyLevel.Value;
            }
            if (defaultSets.HasValue)
            {
                DefaultSets = defaultSets;
            }
            if (defaultReps.HasValue)
            {
                DefaultReps = defaultReps;
            }

            return Result.Updated;
        }
    }

    public enum DifficultyLevel
    {
        Beginner = 1,
        Intermediate = 2,
        Advanced = 3
    }
}
