using GymAssistant_API.Model.Entities.Exercise;
using GymAssistant_API.Model.Results;
using GymAssistant_API.Repository.Interfaces.ExerciseExercises;
using GymAssistant_API.Req_Res.Reqeust.Exercise;
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
            return createExercise.Value;
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
            return getExercise.Value;
        }
        public async Task<Result<ExercisesResponse>> ExercisesBySection(string userId, Guid sectionId, string? searchTerm = null, DifficultyLevel? difficulty = null, CancellationToken ct = default)
        {
            var getExercises = await exercise.GetExercisesBySectionAsync(userId, sectionId, searchTerm, difficulty, ct);
            if (getExercises.IsError)
            {
                logger.LogError("Failed to retrieve exercises: {Error}", getExercises.Errors);
                return getExercises.Errors;
            }
            return getExercises.Value;
        }
        public async Task<Result<List<SectionResponse>>> GetSections(string userId, CancellationToken ct = default)
        {
            var getSections = await exercise.GetSectionsAsync(userId, ct);
            if (getSections.IsError)
            {
                logger.LogError("Failed to retrieve sections: {Error}", getSections.Errors);
                return getSections.Errors;
            }
            return getSections.Value;
        }
        public async Task<Result<SectionResponse>> GetSectionByIdAsync(string userId, Guid sectionId, CancellationToken ct = default)

        {
            var getSection = await exercise.GetSectionByIdAsync(userId, sectionId, ct);
            if (getSection.IsError)
            {
                logger.LogError("Failed to retrieve section {SectionId} for user : {Error}", sectionId, getSection.TopError.Description);
                return getSection.Errors;
            }
            return getSection.Value;
        }
        public async Task<Result<List<SectionGroupResponse>>> AllSectionGroups(string userId, Guid sectionId, CancellationToken ct = default)
        {
            var getGroups = await exercise.AllSectionGroups(userId, sectionId, ct);
            if (getGroups.IsError)
            {
                logger.LogError("Failed to retrieve section groups for section {SectionId} for user : {Error}", sectionId, getGroups.TopError.Description);
                return getGroups.Errors;
            }
            return getGroups.Value;
        }
        public async Task<Result<SectionGroupResponse>> CreateSectionGroup(string userId, SectionGroupReq req, CancellationToken ct = default)

        {
            var createGroup = await exercise.CreateSectionGroup(userId, req.SectionId, req.Name, req.Description, ct);
            if (createGroup.IsError)
            {
                logger.LogError("Failed to create section group {GroupName} for user : {Error}", req.Name, createGroup.TopError.Description);
                return createGroup.Errors;
            }
            return createGroup.Value;
        }
        public async Task<Result<SectionGroupResponse>> AddExerciseToGroup(string userId, Guid groupId, Guid? exerciseId, Guid? customExerciseId, CancellationToken ct = default)
        {
            var addExercise = await exercise.AddExerciseToGroup(userId, groupId, exerciseId, customExerciseId, ct);
            if (addExercise.IsError)
            {
                logger.LogError("Failed to add exercise to group {GroupId} for user : {Error}", groupId, addExercise.TopError.Description);
                return addExercise.Errors;
            }
            return addExercise.Value;
        }
        public async Task<Result<Updated>> UpdateGroup(string userId, Guid groupId, string name, string descripion, CancellationToken ct = default)
        {
            var updateGroup = await exercise.UpdateGroup(userId, groupId, name, descripion, ct);
            if (updateGroup.IsError)
            {
                logger.LogError("Failed to update group {GroupId} for user : {Error}", groupId, updateGroup.TopError.Description);
                return updateGroup.Errors;
            }
            return Result.Updated;
        }
        public async Task<Result<Deleted>> DeleteGroup(string userId, Guid groupId, CancellationToken ct = default)
        {
            var deleteGroup = await exercise.DeleteGroup(userId, groupId, ct);
            if (deleteGroup.IsError)
            {
                logger.LogError("Failed to delete group {GroupId} for user : {Error}", groupId, deleteGroup.TopError.Description);
                return deleteGroup.Errors;
            }
            return Result.Deleted;
        }
        public async Task<Result<Deleted>> DeleteExerciseFromGroup(string userId, Guid groupId, Guid? exerciseId, Guid? customExerciseId,
                                                                           CancellationToken ct = default)
        {
            var deleteExercise = await exercise.DeleteExerciseFromGroup(userId, groupId, exerciseId, customExerciseId, ct);
            if (deleteExercise.IsError)
            {
                logger.LogError("Failed to delete exercise from group {GroupId} for user : {Error}", groupId, deleteExercise.TopError.Description);
                return deleteExercise.Errors;
            }
            return Result.Deleted;
        }

    }
}
