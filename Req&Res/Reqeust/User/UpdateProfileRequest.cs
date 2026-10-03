using GymAssistant_API.Model.Entities.User;
using GymAssistant_API.Resources;
using System.ComponentModel.DataAnnotations;

namespace GymAssistant_API.Req_Res.Reqeust
{
    public record UpdateProfileRequest(
        [Display(Name = LocalizationKeys.Fields.FirstName)]
        [StringLength(50)]
        string? FirstName = default,

        [Display(Name = LocalizationKeys.Fields.LastName)]
        [StringLength(50)]
        string? LastName = default,

        IFormFile? imageFile = default,

        [Display(Name = LocalizationKeys.Fields.Gender)]
        [EnumDataType(typeof(Gender))]
        Gender? Gender = default,

        [Display(Name = LocalizationKeys.Fields.PhoneNumber)]
        [StringLength(50)]
        string? PhoneNumber = default,

        [Display(Name = LocalizationKeys.Fields.BirthDate)]
        [DataType(DataType.Date)]
        DateTime? BirthDate = default,

        [Display(Name = LocalizationKeys.Fields.Height)]
        [Range(50, 300, ErrorMessage = LocalizationKeys.Validation.Range)]
        int? HeightCm = default,

        [Display(Name = LocalizationKeys.Fields.Weight)]
        [Range(20, 400, ErrorMessage = LocalizationKeys.Validation.Range)]
        decimal? WeightKg = default,

        [Display(Name = LocalizationKeys.Fields.Weight)]
        [Range(20, 400, ErrorMessage = LocalizationKeys.Validation.Range)]
        decimal? WeightGoal = default,

        [Display(Name = LocalizationKeys.Fields.BodyFat)]
        [Range(0, 100, ErrorMessage = LocalizationKeys.Validation.Range)]
        decimal? BodyFatPercent = default,

        [Display(Name = LocalizationKeys.Fields.BodyFat)]
        [Range(0, 100, ErrorMessage = LocalizationKeys.Validation.Range)]
        decimal? BodyFatGoal = default,

        [Display(Name = LocalizationKeys.Fields.MuscleMass)]
        [Range(10, 200, ErrorMessage = LocalizationKeys.Validation.Range)]
        decimal? MuscleMassKg = default,

        [Display(Name = LocalizationKeys.Fields.MuscleMass)]
        [Range(10, 200, ErrorMessage = LocalizationKeys.Validation.Range)]
        decimal? MuscleMassGoal = default
    );
}
