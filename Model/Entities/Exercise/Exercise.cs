using GymAssistant_API.Model.Identity;
using GymAssistant_API.Model.Results;
using Microsoft.AspNetCore.Http.HttpResults;

namespace GymAssistant_API.Model.Entities.Exercise
{
    public sealed class Exercise : Entity
    {
        public Guid SectionId { get; private set; }
        public Section Section { get; private set; } = default!;

        public string Name { get; private set; }
        public string? Description { get; private set; }
        public string? Instructions { get; private set; }
        public string? ImageUrl { get; private set; }
        public string? Equipment { get; private set; }
        public DifficultyLevel? DifficultyLevel { get; private set; }
        public int? DefaultSets { get; private set; }
        public int? DefaultReps { get; private set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        private Exercise() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

        private Exercise(Guid id, Guid sectionId, string name, string? description = null,
                       string? instructions = null, string? imageUrl = null,
                       string? equipment = null, DifficultyLevel? difficultyLevel = null,
                       int? defaultSets = null, int? defaultReps = null) : base(id)
        {
            SectionId = sectionId;
            Name = name;
            Description = description;
            Instructions = instructions;
            ImageUrl = imageUrl;
            Equipment = equipment;
            DifficultyLevel = difficultyLevel;
            DefaultSets = defaultSets;
            DefaultReps = defaultReps;
            CreatedAtUtc = DateTime.UtcNow;
        }
        public static Result<Exercise> Create(Guid id, Guid sectionId, string name, string? description = null,
                                            string? instructions = null, string? imageUrl = null,
                                            string? equipment = null, DifficultyLevel? difficultyLevel = null,
                                            int? defaultSets = null, int? defaultReps = null)
        {
            if (sectionId == Guid.Empty)
            {
                return ExerciseErrors.SectionIdRequired;
            }
            if (string.IsNullOrWhiteSpace(name))
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
            var exercise = new Exercise(id, sectionId, name, description, instructions, imageUrl,
                                        equipment, difficultyLevel, defaultSets, defaultReps);
            return exercise;
        }
    }
    public enum DifficultyLevel
    {
        Beginner = 1,
        Intermediate = 2,
        Advanced = 3
    }
}
