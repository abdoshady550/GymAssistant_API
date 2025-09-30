using GymAssistant_API.Model.Entities.User;
using GymAssistant_API.Model.Results;
using GymAssistant_API.Repository.Interfaces.User.Trainer;
using GymAssistant_API.Req_Res.Response.Exercise;
using GymAssistant_API.Req_Res.Response.Trainer;

namespace GymAssistant_API.Handeler.Identity.Trainer
{
    public sealed class TrainerHandler(ILogger<TrainerHandler> logger,
                                      ITrainerService trainerService)
    {
        private readonly ILogger<TrainerHandler> logger = logger;
        private readonly ITrainerService trainerService = trainerService;

        public async Task<Result<TrainerTraineeResponse>> AddTrainee(string trainerId, Guid traineeId,
                                                             CancellationToken ct = default)
        {
            var addTrainee = await trainerService.AddTraineeAsync(trainerId, traineeId, ct);
            if (addTrainee.IsError)
            {
                logger.LogError("Failed to add trainee {TraineeId} for trainer {TrainerId} : {Error}",
                               traineeId, trainerId, addTrainee.TopError.Description);
                return addTrainee.Errors;
            }
            return addTrainee.Value;
        }

        public async Task<Result<List<TraineeData>>> GetTrainees(string trainerId,
                                                                 CancellationToken ct = default)
        {
            var getTrainees = await trainerService.GetTraineesAsync(trainerId, ct);
            if (getTrainees.IsError)
            {
                logger.LogError("Failed to retrieve trainees for trainer {TrainerId} : {Error}",
                               trainerId, getTrainees.TopError.Description);
                return getTrainees.Errors;
            }
            return getTrainees.Value;
        }

        public async Task<Result<TraineeData>> GetTrainee(string trainerId, Guid traineeId,
                                                          CancellationToken ct = default)
        {
            var getTrainee = await trainerService.GetTraineeAsync(trainerId, traineeId, ct);
            if (getTrainee.IsError)
            {
                logger.LogError("Failed to retrieve trainee {TraineeId} for trainer {TrainerId} : {Error}",
                               traineeId, trainerId, getTrainee.TopError.Description);
                return getTrainee.Errors;
            }
            return getTrainee.Value;
        }

        public async Task<Result<Deleted>> RemoveTrainee(string trainerId, Guid traineeId,
                                                         CancellationToken ct = default)
        {
            var removeTrainee = await trainerService.RemoveTraineeAsync(trainerId, traineeId, ct);
            if (removeTrainee.IsError)
            {
                logger.LogError("Failed to remove trainee {TraineeId} for trainer {TrainerId} : {Error}",
                               traineeId, trainerId, removeTrainee.TopError.Description);
                return removeTrainee.Errors;
            }
            return Result.Deleted;
        }

        public async Task<Result<WorkoutSessionRes>> CreateSessionForTrainee(string trainerId,
                                                                             Guid traineeId,
                                                                             DateTime date,
                                                                             string? notes = null,
                                                                             CancellationToken ct = default)
        {
            var createSession = await trainerService.CreateSessionForTraineeAsync(trainerId, traineeId, date, notes, ct);
            if (createSession.IsError)
            {
                logger.LogError("Failed to create session for trainee {TraineeId} by trainer {TrainerId} : {Error}",
                               traineeId, trainerId, createSession.TopError.Description);
                return createSession.Errors;
            }
            return createSession.Value;
        }

        public async Task<Result<List<WorkoutSessionRes>>> GetTraineeSessions(string trainerId,
                                                                              Guid traineeId,
                                                                              int pageSize,
                                                                              int pageNumber,
                                                                              CancellationToken ct = default)
        {
            var getSessions = await trainerService.GetTraineeSessionsAsync(trainerId, traineeId, pageSize, pageNumber, ct);
            if (getSessions.IsError)
            {
                logger.LogError("Failed to retrieve sessions for trainee {TraineeId} by trainer {TrainerId} : {Error}",
                               traineeId, trainerId, getSessions.TopError.Description);
                return getSessions.Errors;
            }
            return getSessions.Value;
        }

        public async Task<Result<WorkoutSessionRes>> GetTraineeSession(string trainerId,
                                                                       Guid traineeId,
                                                                       Guid sessionId,
                                                                       CancellationToken ct = default)
        {
            var getSession = await trainerService.GetTraineeSessionAsync(trainerId, traineeId, sessionId, ct);
            if (getSession.IsError)
            {
                logger.LogError("Failed to retrieve session {SessionId} for trainee {TraineeId} by trainer {TrainerId} : {Error}",
                               sessionId, traineeId, trainerId, getSession.TopError.Description);
                return getSession.Errors;
            }
            return getSession.Value;
        }

        public async Task<Result<TraineeProgressData>> GetTraineeProgress(string trainerId,
                                                                          Guid traineeId,
                                                                          int days,
                                                                          Guid sectionId = default,
                                                                          CancellationToken ct = default)
        {
            var getProgress = await trainerService.GetTraineeProgressAsync(trainerId, traineeId, days, sectionId, ct);
            if (getProgress.IsError)
            {
                logger.LogError("Failed to retrieve progress for trainee {TraineeId} by trainer {TrainerId} : {Error}",
                               traineeId, trainerId, getProgress.TopError.Description);
                return getProgress.Errors;
            }
            return getProgress.Value;
        }

        public async Task<Result<TrainerDashboardData>> GetTrainerDashboard(string trainerId,
                                                                            CancellationToken ct = default)
        {
            var getDashboard = await trainerService.GetTrainerDashboardAsync(trainerId, ct);
            if (getDashboard.IsError)
            {
                logger.LogError("Failed to retrieve dashboard for trainer {TrainerId} : {Error}",
                               trainerId, getDashboard.TopError.Description);
                return getDashboard.Errors;
            }
            return getDashboard.Value;
        }
    }
}
