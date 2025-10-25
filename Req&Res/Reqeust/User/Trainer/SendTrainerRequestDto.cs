namespace GymAssistant_API.Req_Res.Reqeust.User.Trainer
{
    public record SendTrainerRequestDto(
         string TraineeId,
         string? Message = null
     );
    public record SendUserRequestDto(
      string TrainerId,
      string? Message = null
  );
}
