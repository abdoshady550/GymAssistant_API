using GymAssistant_API.Model.Entities.Exercise;
using ExerciseEntity = GymAssistant_API.Model.Entities.Exercise.Exercise;


namespace GymAssistant_API.Req_Res.Response.Records
{
    public class PersonalRecordResponse
    {
        public Guid Id { get; set; }
        public Guid ClientProfileId { get; set; }
        public Guid? ExerciseId { get; set; }

        public Guid? UserExerciseId { get; set; }
        public RecordType RecordType { get; set; }
        public decimal Value { get; set; }
        public Guid WorkoutSessionId { get; set; }
        public DateTimeOffset CreatedAtUtc { get; set; }

        // Related entity information (flattened to avoid circular references)
        public ExerciseInfo? Exercise { get; set; }
        public UserExerciseInfo? UserExercise { get; set; }
        public WorkoutSessionInfo WorkoutSession { get; set; } = new();
    }

    public class ExerciseInfo
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string SectionName { get; set; } = string.Empty;
        public DifficultyLevel? DifficultyLevel { get; set; }
    }

    public class UserExerciseInfo
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }

    public class WorkoutSessionInfo
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public bool IsCompleted { get; set; }
        public int? DurationMinutes { get; set; }
        public string? Notes { get; set; }
    }

    // Extension method to convert entity to DTO
    public static class PersonalRecordExtensions
    {
        public static PersonalRecordResponse ToResponse(this PersonalRecord record)
        {
            return new PersonalRecordResponse
            {
                Id = record.Id,
                ClientProfileId = record.ClientProfileId,
                ExerciseId = record.ExerciseId,
                UserExerciseId = record.UserExerciseId,
                RecordType = record.RecordType,
                Value = record.Value,
                WorkoutSessionId = record.WorkoutSessionId,
                CreatedAtUtc = record.CreatedAtUtc,
                Exercise = record.Exercise?.ToExerciseInfo(),
                UserExercise = record.UserExercise?.ToUserExerciseInfo(),
                WorkoutSession = record.WorkoutSession.ToWorkoutSessionInfo()
            };
        }

        public static ExerciseInfo ToExerciseInfo(this ExerciseEntity exercise)
        {
            return new ExerciseInfo
            {
                Id = exercise.Id,
                Name = exercise.Name,
                Description = exercise.Description,
                SectionName = exercise.Section?.Name ?? "Unknown Section",
                DifficultyLevel = exercise.DifficultyLevel
            };
        }

        public static UserExerciseInfo ToUserExerciseInfo(this UserExercise userExercise)
        {
            return new UserExerciseInfo
            {
                Id = userExercise.Id,
                Name = userExercise.Name,
                Description = userExercise.Description
            };
        }

        public static WorkoutSessionInfo ToWorkoutSessionInfo(this WorkoutSession session)
        {
            return new WorkoutSessionInfo
            {
                Id = session.Id,
                Date = session.Date,
                StartTime = session.StartTime,
                EndTime = session.EndTime,
                IsCompleted = session.IsCompleted,
                DurationMinutes = session.DurationMinutes,
                Notes = session.Notes
            };
        }
    }
}

