namespace GymAssistant_API.Req_Res.Response.Progress
{
    public record StatesRes(
        int? workoutsCount = default,
        int? customExercisesCount = default,
        int? recordsCount = default,
        int? daysSinceLastWorkout = default,
        int? daysSinceLastRecord = default,
        double? averageWorkoutDuration = default,
        decimal? averageWorkoutsPerWeek = default,
        decimal? personalBestWeight = default,
        int? personalBestReps = default,
        decimal? personalBestVolume = default
        );
}
