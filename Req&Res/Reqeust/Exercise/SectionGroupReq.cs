namespace GymAssistant_API.Req_Res.Reqeust.Exercise
{
    public record SectionGroupReq(
        Guid SectionId,
        string Name,
        string? Description = null
        )
    {
    }
}
