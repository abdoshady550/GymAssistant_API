namespace GymAssistant_API.Req_Res.Reqeust.Exercise
{
    public record class CustomExerciseReq(
        string Name,
        string? Description = default,
        string? Instructions = default,
        string? Equipment = default,
        IFormFile? ImageUrl = default
    );

}
