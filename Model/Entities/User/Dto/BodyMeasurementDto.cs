namespace GymAssistant_API.Model.Entities.User.Dto
{
    public record BodyMeasurementDto(
     Guid? Id = default,
     decimal? WeightKg = default,
     decimal? WeightGoal = default,
     decimal? MuscleMassKg = default,
     decimal? MuscleMassGoal = default,
     decimal? BodyFatPercent = default,
     decimal? BodyFatGoal = default,
     DateTimeOffset? CreatedAtUtc = default);

}
