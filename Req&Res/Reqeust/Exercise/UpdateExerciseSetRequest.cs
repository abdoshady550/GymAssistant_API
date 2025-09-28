namespace GymAssistant_API.Req_Res.Reqeust.Exercises
{
    public record UpdateExerciseSetRequest(int Reps,
                                           decimal WeightKg,
                                           int? RestTimeSeconds = null,
                                           string? Notes = null);

}
