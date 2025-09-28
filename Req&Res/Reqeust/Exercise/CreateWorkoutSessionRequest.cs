
namespace GymAssistant_API.Req_Res.Reqeust.Exercises
{
    public record CreateWorkoutSessionRequest(

        DateTime Date,
        string? Notes = null,
        Guid? TraineeId = null);

}
