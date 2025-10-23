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
        public static SectionResponse FromEntity(string userId, Section section)
        {
            var userCustomExercises = section.UserExercise
                                    .Where(u => u.UserId == userId)
                                    .Count();

            return new SectionResponse(
                section.Id,
                section.Name,
                section.Description,
                section.SectionGroup.Select(SectionGroupResponse.FromEntity).ToList(),
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
        public static SectionGroupResponse FromEntity(SectionGroup group)
        {
            return new SectionGroupResponse(
                group.Id,
                group.SectionId,
                group.Name,
                group.Description,
                group.Exercises.Select(ExerciseResponse.FromEntity).ToList(),
                group.UserExercise.Select(CustomExerciseRes.FromEntity).ToList(),
                group.CreatedAtUtc
                );
        }
    };
}
