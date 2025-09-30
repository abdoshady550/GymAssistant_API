using GymAssistant_API.Model.Entities.Exercise;
using GymAssistant_API.Model.Results;
using GymAssistant_API.Repository.Interfaces.ExerciseExercises;
using GymAssistant_API.Req_Res.Reqeust.Exercises;
using GymAssistant_API.Req_Res.Response;
using GymAssistant_API.Req_Res.Response.Exercise;
using System;

namespace GymAssistant_API.Handeler.Exercise
{
    public sealed class ExerciseHandler(ILogger<ExerciseHandler> logger,
                                        IExercise exercise)
    {
        private readonly ILogger<ExerciseHandler> logger = logger;
        private readonly IExercise exercise = exercise;

        public async Task<Result<ExerciseResponse>> CreateExercise(Guid sectionId, ExerciseReq req,
                                                               CancellationToken ct = default)
        {
            var createExercise = await exercise.CreateExerciseAsync(sectionId,
                                                                    req.Name,
                                                                    req.Description,
                                                                    req.Instructions,
                                                                    req.ImageFile,
                                                                    req.Equipment,
                                                                    req.DifficultyLevel,
                                                                    req.DefaultSets,
                                                                    req.DefaultReps,
                                                                    ct);
            if (createExercise.IsError)
            {
                logger.LogError("Failed to create exercise {ExerciseName} : {Error}", req.Name, createExercise.Errors);
                return createExercise.Errors;
            }
            return createExercise;
        }
        public async Task<Result<Updated>> UpdateExercise(Guid exerciseId, Guid sectionId, UpdateExerciseReq req,
                                                               CancellationToken ct = default)
        {
            var updateExercise = await exercise.UpdateExerciseAsync(exerciseId,
                                                                    sectionId,
                                                                    req.Name,
                                                                    req.Description,
                                                                    req.Instructions,
                                                                    req.ImageFile,
                                                                    req.Equipment,
                                                                    req.DifficultyLevel,
                                                                    req.DefaultSets,
                                                                    req.DefaultReps,
                                                                    ct);
            if (updateExercise.IsError)
            {
                logger.LogError("Failed to update exercise {ExerciseId} : {Error}", exerciseId, updateExercise.Errors);
                return updateExercise.Errors;
            }
            return Result.Updated;
        }
        public async Task<Result<Deleted>> DeleteExercise(Guid exerciseId,
                                                               CancellationToken ct = default)
        {
            var deleteExercise = await exercise.DeleteExerciseAsync(exerciseId, ct);
            if (deleteExercise.IsError)
            {
                logger.LogError("Failed to delete exercise {ExerciseId} : {Error}", exerciseId, deleteExercise.Errors);
                return deleteExercise.Errors;
            }
            return Result.Deleted;
        }
        public async Task<Result<ExerciseResponse>> GetExerciseById(Guid exerciseId,
                                                               CancellationToken ct = default)
        {
            var getExercise = await exercise.GetExerciseAsync(exerciseId, ct);
            if (getExercise.IsError)
            {
                logger.LogError("Failed to retrieve custom exercise {ExerciseId} for user : {Error}", exerciseId, getExercise.Errors);
                return getExercise.Errors;
            }
            return getExercise;
        }
        public async Task<Result<List<ExerciseResponse>>> ExercisesBySection(Guid sectionId, DifficultyLevel? difficulty = null, CancellationToken ct = default)
        {
            var getExercises = await exercise.GetExercisesBySectionAsync(sectionId, difficulty, ct);
            if (getExercises.IsError)
            {
                logger.LogError("Failed to retrieve exercises: {Error}", getExercises.Errors);
                return getExercises.Errors;
            }
            return getExercises;
        }
        public async Task<Result<List<SectionResponse>>> GetSections(CancellationToken ct = default)
        {
            var getSections = await exercise.GetSectionsAsync(ct);
            if (getSections.IsError)
            {
                logger.LogError("Failed to retrieve sections: {Error}", getSections.Errors);
                return getSections.Errors;
            }
            return getSections;
        }
    }
}
