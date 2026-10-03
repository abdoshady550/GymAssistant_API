using GymAssistant_API.Model.Entities.Exercise;
using ExerciseEntity = GymAssistant_API.Model.Entities.Exercise.Exercise;

namespace GymAssistant_API.Req_Res.Response.Exercise
{
    public record SectionResponse(
       Guid Id,
       string Name,
       string? Description = null,
       List<SectionGroupResponse>? SectionGroup = null,
       DateTimeOffset? CreatedAtUtc = null,
       int? ExerciseNumber = 0,
       int? CustomExerciseNumber = 0,
       int? AllExerciseNumber = 0

   )
    {
        public static SectionResponse FromEntity(string userId, Section section, GymAssistant_API.Resources.ISeedDataLocalizer? seedLocalizer = null)
        {
            var userCustomExercises = section.UserExercise
                                    .Where(u => u.UserId == userId)
                                    .Count();

            var name = seedLocalizer != null
                ? seedLocalizer.SectionName(section.Id, section.Name)
                : section.Name;

            var description = seedLocalizer != null
                ? seedLocalizer.SectionDescription(section.Id, section.Description)
                : section.Description;

            return new SectionResponse(
                section.Id,
                name,
                description,
                section.SectionGroup.Select(sg => SectionGroupResponse.FromEntity(sg, seedLocalizer)).ToList(),
                section.CreatedAtUtc,
                section.Exercises.Count(),
                userCustomExercises,
                section.Exercises.Count() + section.UserExercise.Count()
                );
        }

    };
    public record SectionGroupResponse(
       Guid Id,
       Guid sectionId,
       string Name,
       string? Description = null,
       List<ExerciseResponse>? Exercise = null,
       List<CustomExerciseRes>? CustomExercise = null,
       DateTimeOffset? CreatedAtUtc = null
   )
    {
        public static SectionGroupResponse FromEntity(SectionGroup group, GymAssistant_API.Resources.ISeedDataLocalizer? seedLocalizer = null)
        {
            return new SectionGroupResponse(
                group.Id,
                group.SectionId,
                group.Name,
                group.Description,
                group.Exercises.Select(e => ExerciseResponse.FromEntity(e, seedLocalizer)).ToList(),
                group.UserExercise.Select(CustomExerciseRes.FromEntity).ToList(),
                group.CreatedAtUtc
                );
        }
    };
}
