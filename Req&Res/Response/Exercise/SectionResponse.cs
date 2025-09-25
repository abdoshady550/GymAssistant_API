namespace GymAssistant_API.Req_Res.Response.Exercise
{
    public record SectionResponse(
       Guid Id,
       string Name,
       string? Description = null,
       DateTimeOffset? CreatedAtUtc = null
   );
}
