namespace GymAssistant_API.Model.Entities.User.Dto
{
    public record BodyMeasurementDto(
     Guid? Id = default,
     decimal? WeightKg = default,
     decimal? MuscleMassKg = default,
     decimal? BodyFatPercent = default,
     DateTimeOffset? CreatedAtUtc = default);

}
