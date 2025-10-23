namespace GymAssistant_API.Req_Res.Reqeust.Exercises
{
    public record UpdateExerciseSetRequest(int? Reps = null,
                                           decimal? WeightKg = null,
                                           int? RestTimeSeconds = null,
                                           string? Notes = null);

}
