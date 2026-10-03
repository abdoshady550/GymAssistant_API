using GymAssistant_API.Model.Entities.Exercise;
using GymAssistant_API.Model.Results;
using GymAssistant_API.Repository.Interfaces.Exercise;
using GymAssistant_API.Req_Res.Reqeust.Predefined;
using GymAssistant_API.Req_Res.Response.Predefined;

namespace GymAssistant_API.Handeler.Exercise
{
    public sealed class AdminPredefinedHandler(ILogger<AdminPredefinedHandler> logger, IPredefinedDataService service)
    {
        private readonly ILogger<AdminPredefinedHandler> _logger = logger;
        private readonly IPredefinedDataService _service = service;

        // Sections
        public Task<Result<AdminSectionRes>> CreateSection(AdminSectionReq req, CancellationToken ct = default)
            => _service.CreateSectionAsync(req, ct);

        public Task<Result<Updated>> UpdateSection(Guid id, AdminSectionReq req, CancellationToken ct = default)
            => _service.UpdateSectionAsync(id, req, ct);

        public Task<Result<Deleted>> DeleteSection(Guid id, CancellationToken ct = default)
            => _service.DeleteSectionAsync(id, ct);

        public Task<Result<List<AdminSectionRes>>> GetAdminSections(CancellationToken ct = default)
            => _service.GetAdminSectionsAsync(ct);

        // Exercises
        public Task<Result<AdminExerciseRes>> CreateExercise(AdminExerciseReq req, CancellationToken ct = default)
            => _service.CreateExerciseAsync(req, ct);

        public Task<Result<Updated>> UpdateExercise(Guid id, AdminExerciseReq req, CancellationToken ct = default)
            => _service.UpdateExerciseAsync(id, req, ct);

        public Task<Result<Deleted>> DeleteExercise(Guid id, CancellationToken ct = default)
            => _service.DeleteExerciseAsync(id, ct);

        public Task<Result<List<AdminExerciseRes>>> GetAdminExercises(Guid? sectionId = null, CancellationToken ct = default)
            => _service.GetAdminExercisesAsync(sectionId, ct);

        // Workdays
        public Task<Result<AdminWorkdayRes>> CreateWorkday(AdminWorkdayReq req, CancellationToken ct = default)
            => _service.CreateWorkdayAsync(req, ct);

        public Task<Result<Updated>> UpdateWorkday(Guid id, AdminWorkdayReq req, CancellationToken ct = default)
            => _service.UpdateWorkdayAsync(id, req, ct);

        public Task<Result<Deleted>> DeleteWorkday(Guid id, CancellationToken ct = default)
            => _service.DeleteWorkdayAsync(id, ct);

        public Task<Result<List<AdminWorkdayRes>>> GetAdminWorkdays(CancellationToken ct = default)
            => _service.GetAdminWorkdaysAsync(ct);

        // Predefined Sessions
        public Task<Result<AdminPredefinedSessionRes>> CreatePredefinedSession(AdminPredefinedSessionReq req, CancellationToken ct = default)
            => _service.CreatePredefinedSessionAsync(req, ct);

        public Task<Result<Updated>> UpdatePredefinedSession(Guid id, AdminPredefinedSessionReq req, CancellationToken ct = default)
            => _service.UpdatePredefinedSessionAsync(id, req, ct);

        public Task<Result<Deleted>> DeletePredefinedSession(Guid id, CancellationToken ct = default)
            => _service.DeletePredefinedSessionAsync(id, ct);

        public Task<Result<List<AdminPredefinedSessionRes>>> GetAdminPredefinedSessions(Guid? workdayId = null, CancellationToken ct = default)
            => _service.GetAdminPredefinedSessionsAsync(workdayId, ct);
    }
}
