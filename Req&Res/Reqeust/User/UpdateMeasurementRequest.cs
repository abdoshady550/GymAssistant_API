namespace GymAssistant_API.Req_Res.Reqeust.User
{
    public record UpdateMeasurementRequest(
        decimal? WeightKg,
        decimal? WeightGoal,
        decimal? BodyFatPercent,
        decimal? BodyFatGoal,
        decimal? MuscleMassKg,
        decimal? MuscleMassGoal
        );
}
