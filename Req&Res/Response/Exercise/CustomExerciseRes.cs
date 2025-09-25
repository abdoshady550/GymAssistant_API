namespace GymAssistant_API.Req_Res.Response
{
    public record CustomExerciseRes(
      Guid Id,
      string UserId,
      string Name,
      string? Description = default,
      string? Instructions = default,
      string? Equipment = default,
      string? ImageUrl = default,
      DateTimeOffset? CreatedAtUtc = default
  );
}
