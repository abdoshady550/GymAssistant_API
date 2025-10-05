namespace GymAssistant_API.Req_Res.Reqeust.Exercises
{
    public record class CustomExerciseReq(
        string Name,
        string? Description = default,
        string? Instructions = default,
        string? Equipment = default,
        IFormFile? ImageFile = default
    );

}
