using GymAssistant_API.Model.Entities.Exercise;
using GymAssistant_API.Model.Results;
using GymAssistant_API.Repository.Interfaces.ExerciseExercises;
using GymAssistant_API.Req_Res.Reqeust.Exercises;
using GymAssistant_API.Req_Res.Response;

namespace GymAssistant_API.Handeler.Exercise
{
    public sealed class CustomExerciseHandler(ILogger<CustomExerciseHandler> logger,
                                              IExercise exercise)
    {
        private readonly ILogger<CustomExerciseHandler> logger = logger;
        private readonly IExercise exercise = exercise;

        public async Task<Result<CustomExerciseRes>> CreateCustomExercise(string userId,
                                                                    CustomExerciseReq req,
                                                                     CancellationToken ct = default)
        {
            var createExercise = await exercise.CreateCustomExerciseAsync(userId,
                                                                          req.Name,
                                                                          req.Description,
                                                                          req.Instructions,
                                                                          req.Equipment,
                                                                          req.ImageUrl,
                                                                          ct);
            if (createExercise.IsError)
            {
                logger.LogError("Failed to create custom exercise for user {UserId}: {Error}", userId, createExercise.Errors);
                return createExercise.Errors;
            }
            return createExercise;
        }
        public async Task<Result<Deleted>> DeleteCustomExercise(string userId,
                                                              Guid exerciseId,
                                                              CancellationToken ct = default)
        {
            var deleteExercise = await exercise.DeleteCustomExerciseAsync(userId, exerciseId, ct);
            if (deleteExercise.IsError)
            {
                logger.LogError("Failed to delete custom exercise {ExerciseId} for user {UserId}: {Error}", exerciseId, userId, deleteExercise.Errors);
                return deleteExercise.Errors;
            }
            return Result.Deleted;
        }
        public async Task<Result<List<CustomExerciseRes>>> GetCustomExercises(string userId,
                                                                     CancellationToken ct = default)
        {
            var getExercises = await exercise.GetCustomExercisesAsync(userId, ct);
            if (getExercises.IsError)
            {
                logger.LogError("Failed to retrieve custom exercises for user {UserId}: {Error}", userId, getExercises.Errors);
                return getExercises.Errors;
            }
            return getExercises;
        }
        public async Task<Result<CustomExerciseRes>> GetCustomExercise(string userId,
                                                            Guid exerciseId,
                                                            CancellationToken ct = default)
        {
            var getExercise = await exercise.GetCustomExerciseAsync(userId, exerciseId, ct);
            if (getExercise.IsError)
            {
                logger.LogError("Failed to retrieve custom exercise {ExerciseId} for user {UserId}: {Error}", exerciseId, userId, getExercise.Errors);
                return getExercise.Errors;
            }
            return getExercise;
        }
        public async Task<Result<Updated>> UpdateCustomExercise(string userId,
                                                              Guid exerciseId,
                                                              CustomExerciseReq req,
                                                              CancellationToken ct = default)
        {
            var updateExercise = await exercise.UpdateCustomExerciseAsync(userId,
                                                                          exerciseId,
                                                                          req.Name,
                                                                          req.Description,
                                                                          req.Instructions,
                                                                          req.Equipment,
                                                                          req.ImageUrl,
                                                                          ct);
            if (updateExercise.IsError)
            {
                logger.LogError("Failed to update custom exercise {ExerciseId} for user {UserId}: {Error}", exerciseId, userId, updateExercise.Errors);
                return updateExercise.Errors;
            }
            return Result.Updated;
        }

    }
}
