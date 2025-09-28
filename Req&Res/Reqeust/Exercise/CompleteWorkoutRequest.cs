namespace GymAssistant_API.Req_Res.Reqeust.Exercises
{
    public record CompleteWorkoutRequest(DateTime EndTime, string? Notes = null);
}
