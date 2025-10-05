namespace GymAssistant_API.Req_Res.Reqeust.User.Trainer
{
    public record SendTrainerRequestDto(
         Guid TraineeId,
         string? Message = null
     );
}
