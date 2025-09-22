using GymAssistant_API.Model.Entities.User;
using System;
using System.ComponentModel.DataAnnotations;

namespace GymAssistant_API.Req_Res.Reqeust
{
    public record CreateProfileRequest(
        [Required(ErrorMessage = "First name is required")]
        [StringLength(50, ErrorMessage = "First name cannot exceed 50 characters")]
        string FirstName,

        [Required(ErrorMessage = "Last name is required")]
        [StringLength(50, ErrorMessage = "Last name cannot exceed 50 characters")]
        string LastName,

        [Required(ErrorMessage = "Gender is required")]
        [EnumDataType(typeof(Gender))]
        Gender Gender
    );

    public record MeasurementRequest(
        [Required(ErrorMessage = "Weight is required")]
        [Range(20, 400, ErrorMessage = "Weight must be between 20 and 400 kg")]
        decimal WeightKg,

        [Range(0, 100, ErrorMessage = "Body fat percentage must be between 0 and 100")]
        decimal? BodyFatPercent = null,

        [Range(10, 200, ErrorMessage = "Muscle mass must be between 10 and 200 kg")]
        decimal? MuscleMassKg = null
    );
}
