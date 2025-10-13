namespace GymAssistant_API.Req_Res.Response.Exercise
{
    public record ExercisesResponse(
        List<ExerciseResponse> Exercise
        , List<CustomExerciseRes> CustomExercise
        );
}
