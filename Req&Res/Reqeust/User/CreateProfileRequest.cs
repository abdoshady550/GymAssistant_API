using GymAssistant_API.Model.Entities.User;
using GymAssistant_API.Resources;
using System.ComponentModel.DataAnnotations;

namespace GymAssistant_API.Req_Res.Reqeust
{
    public record CreateProfileRequest(
        [Display(Name = LocalizationKeys.Fields.FirstName)]
        [Required(ErrorMessage = LocalizationKeys.Validation.Required)]
        [StringLength(50)]
        string FirstName,

        [Display(Name = LocalizationKeys.Fields.LastName)]
        [Required(ErrorMessage = LocalizationKeys.Validation.Required)]
        [StringLength(50)]
        string LastName,

        [Display(Name = LocalizationKeys.Fields.Gender)]
        [Required(ErrorMessage = LocalizationKeys.Validation.Required)]
        [EnumDataType(typeof(Gender))]
        Gender Gender
    );

    public record MeasurementRequest(
        [Display(Name = LocalizationKeys.Fields.Weight)]
        [Range(20, 400, ErrorMessage = LocalizationKeys.Validation.Range)]
        decimal? WeightKg,

        [Display(Name = LocalizationKeys.Fields.Weight)]
        [Range(20, 400, ErrorMessage = LocalizationKeys.Validation.Range)]
        decimal WeightGoal,

        [Display(Name = LocalizationKeys.Fields.BodyFat)]
        [Range(0, 100, ErrorMessage = LocalizationKeys.Validation.Range)]
        decimal? BodyFatPercent = null,

        [Display(Name = LocalizationKeys.Fields.BodyFat)]
        [Range(0, 100, ErrorMessage = LocalizationKeys.Validation.Range)]
        decimal? BodyFatGoal = null,

        [Display(Name = LocalizationKeys.Fields.MuscleMass)]
        [Range(10, 200, ErrorMessage = LocalizationKeys.Validation.Range)]
        decimal? MuscleMassKg = null,

        [Display(Name = LocalizationKeys.Fields.MuscleMass)]
        [Range(10, 200, ErrorMessage = LocalizationKeys.Validation.Range)]
        decimal? MuscleMassGoal = null
    );
}
