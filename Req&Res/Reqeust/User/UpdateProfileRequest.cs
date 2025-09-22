using GymAssistant_API.Model.Entities.User;
using System.ComponentModel.DataAnnotations;

namespace GymAssistant_API.Req_Res.Reqeust
{
    public record UpdateProfileRequest(

        [StringLength(50)]
        string FirstName,


        [StringLength(50)]
        string LastName,


        [EnumDataType(typeof(Gender))]
        Gender Gender,

        [DataType(DataType.Date)]
        DateTime? BirthDate,

        [Range(50, 300, ErrorMessage = "Height must be between 50 and 300 cm")]
        int? HeightCm,


        [Range(20, 400, ErrorMessage = "Weight must be between 20 and 400 kg")]
        decimal WeightKg,

        [Range(0, 100, ErrorMessage = "Body fat percentage must be between 0 and 100")]
        decimal? BodyFatPercent = null,

        [Range(10, 200, ErrorMessage = "Muscle mass must be between 10 and 200 kg")]
        decimal? MuscleMassKg = null

        );


}
