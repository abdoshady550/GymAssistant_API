namespace GymAssistant_API.Req_Res.Reqeust.User
{
    public record UpdateMeasurementRequest(
        decimal WeightKg,
        decimal? BodyFatPercent,
        decimal? MuscleMassKg);
}
