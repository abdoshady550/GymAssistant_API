using GymAssistant_API.Model.Entities.Exercise;

namespace GymAssistant_API.Req_Res.Response.Predefined
{
    // ==========================================
    // Admin Responses (Full Bilingual View)
    // ==========================================

    public record AdminSectionRes(
        Guid Id,
        string NameEn,
        string? NameAr,
        string? DescriptionEn,
        string? DescriptionAr,
        string? ImageUrl,
        int ExercisesCount,
        DateTimeOffset CreatedAtUtc
    );

    public record AdminExerciseRes(
        Guid Id,
        Guid SectionId,
        string? SectionNameEn,
        string? SectionNameAr,
        string NameEn,
        string? NameAr,
        string? DescriptionEn,
        string? DescriptionAr,
        string? InstructionsEn,
        string? InstructionsAr,
        string? EquipmentEn,
        string? EquipmentAr,
        string? ImageUrl,
        DifficultyLevel? DifficultyLevel,
        int? DefaultSets,
        int? DefaultReps,
        DateTimeOffset CreatedAtUtc
    );

    public record AdminWorkdayRes(
        Guid Id,
        string NameEn,
        string? NameAr,
        string? DescriptionEn,
        string? DescriptionAr,
        string? ImageUrl,
        int? DayNumber,
        bool IsActive,
        int SessionsCount,
        DateTimeOffset CreatedAtUtc
    );

    public record AdminSessionExerciseRes(
        Guid Id,
        Guid ExerciseId,
        string ExerciseNameEn,
        string? ExerciseNameAr,
        string? ImageUrl,
        int Order,
        int DefaultSets,
        int DefaultReps,
        int? DefaultRestTimeSeconds
    );

    public record AdminPredefinedSessionRes(
        Guid Id,
        Guid? WorkdayId,
        string? WorkdayNameEn,
        string? WorkdayNameAr,
        string NameEn,
        string? NameAr,
        string? DescriptionEn,
        string? DescriptionAr,
        string? ImageUrl,
        DifficultyLevel? DifficultyLevel,
        int? EstimatedDurationMinutes,
        bool IsActive,
        List<AdminSessionExerciseRes> Exercises,
        DateTimeOffset CreatedAtUtc
    );

    // ==========================================
    // Client Responses (Culture-Localized View)
    // ==========================================

    public record PredefinedSectionDto(
        Guid Id,
        string Name,
        string? Description,
        string? ImageUrl,
        string NameEn,
        string? NameAr,
        int ExercisesCount
    );

    public record PredefinedExerciseDto(
        Guid Id,
        Guid SectionId,
        string SectionName,
        string Name,
        string? Description,
        string? Instructions,
        string? Equipment,
        string? ImageUrl,
        DifficultyLevel? DifficultyLevel,
        int? DefaultSets,
        int? DefaultReps,
        string NameEn,
        string? NameAr
    );

    public record PredefinedSessionExerciseDto(
        Guid Id,
        Guid ExerciseId,
        string ExerciseName,
        string? ExerciseDescription,
        string? ExerciseImageUrl,
        string? Equipment,
        int Order,
        int DefaultSets,
        int DefaultReps,
        int? DefaultRestTimeSeconds
    );

    public record PredefinedWorkoutSessionDto(
        Guid Id,
        Guid? WorkdayId,
        string? WorkdayName,
        string Name,
        string? Description,
        string? ImageUrl,
        DifficultyLevel? DifficultyLevel,
        int? EstimatedDurationMinutes,
        string NameEn,
        string? NameAr,
        List<PredefinedSessionExerciseDto> Exercises
    );

    public record PredefinedWorkdayDto(
        Guid Id,
        string Name,
        string? Description,
        string? ImageUrl,
        int? DayNumber,
        string NameEn,
        string? NameAr,
        List<PredefinedWorkoutSessionDto> Sessions
    );
}
