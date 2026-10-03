using GymAssistant_API.Model.Entities.Exercise;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace GymAssistant_API.Req_Res.Reqeust.Predefined
{
    public class AdminSectionReq
    {
        [Required]
        [MaxLength(200)]
        public string NameEn { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? NameAr { get; set; }

        [MaxLength(1000)]
        public string? DescriptionEn { get; set; }

        [MaxLength(1000)]
        public string? DescriptionAr { get; set; }

        public IFormFile? ImageFile { get; set; }
    }

    public class AdminExerciseReq
    {
        [Required]
        public Guid SectionId { get; set; }

        [Required]
        [MaxLength(200)]
        public string NameEn { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? NameAr { get; set; }

        [MaxLength(2000)]
        public string? DescriptionEn { get; set; }

        [MaxLength(2000)]
        public string? DescriptionAr { get; set; }

        [MaxLength(4000)]
        public string? InstructionsEn { get; set; }

        [MaxLength(4000)]
        public string? InstructionsAr { get; set; }

        [MaxLength(500)]
        public string? EquipmentEn { get; set; }

        [MaxLength(500)]
        public string? EquipmentAr { get; set; }

        public DifficultyLevel? DifficultyLevel { get; set; }

        public int? DefaultSets { get; set; }

        public int? DefaultReps { get; set; }

        public IFormFile? ImageFile { get; set; }
    }

    public class AdminWorkdayReq
    {
        [Required]
        [MaxLength(200)]
        public string NameEn { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? NameAr { get; set; }

        [MaxLength(1000)]
        public string? DescriptionEn { get; set; }

        [MaxLength(1000)]
        public string? DescriptionAr { get; set; }

        public int? DayNumber { get; set; }

        public bool? IsActive { get; set; } = true;

        public IFormFile? ImageFile { get; set; }
    }

    public class AdminPredefinedSessionReq
    {
        public Guid? WorkdayId { get; set; }

        [Required]
        [MaxLength(200)]
        public string NameEn { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? NameAr { get; set; }

        [MaxLength(2000)]
        public string? DescriptionEn { get; set; }

        [MaxLength(2000)]
        public string? DescriptionAr { get; set; }

        public DifficultyLevel? DifficultyLevel { get; set; }

        public int? EstimatedDurationMinutes { get; set; }

        public bool? IsActive { get; set; } = true;

        public IFormFile? ImageFile { get; set; }

        /// <summary>
        /// JSON array of SessionExerciseItemReq (e.g. [{"exerciseId":"...","order":1,"defaultSets":3,"defaultReps":10,"defaultRestTimeSeconds":60}])
        /// </summary>
        public string? ExercisesJson { get; set; }
    }

    public class SessionExerciseItemReq
    {
        public Guid ExerciseId { get; set; }
        public int Order { get; set; } = 1;
        public int DefaultSets { get; set; } = 3;
        public int DefaultReps { get; set; } = 10;
        public int? DefaultRestTimeSeconds { get; set; } = 60;
    }

    public class PickSessionToWorkoutReq
    {
        [Required]
        public DateTime Date { get; set; }

        public string? Notes { get; set; }

        public Guid? TraineeId { get; set; }
    }
}
