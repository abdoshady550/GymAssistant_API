using GymAssistant_API.Model.Entities.User;
using GymAssistant_API.Model.Entities.User.Dto;
using System.Runtime.CompilerServices;

namespace GymAssistant_API.Req_Res.Response
{
    public class ProfileResponse
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public Gender? Gender { get; set; } = default;
        public UserRole? Role { get; set; } = default;
        public int? HeightCm { get; set; } = default;
        public decimal? LastWeightKg { get; set; } = default;
        public decimal? LastMuscleMassKgdecimal { get; set; } = default;
        public decimal? LastBodyFatPercent { get; set; } = default;

        public List<BodyMeasurementDto> bodyMeasurementsHistory { get; set; } = new();

    }
}
