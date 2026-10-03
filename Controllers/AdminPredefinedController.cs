using Asp.Versioning;
using GymAssistant_API.Handeler.Exercise;
using GymAssistant_API.Model.Results;
using GymAssistant_API.Req_Res.Reqeust.Predefined;
using GymAssistant_API.Req_Res.Response.Predefined;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymAssistant_API.Controllers
{
    [Route("api/admin/predefined")]
    [ApiVersionNeutral]
    [Authorize(Roles = "Admin")]
    public sealed class AdminPredefinedController(AdminPredefinedHandler handler) : ApiController
    {
        private readonly AdminPredefinedHandler _handler = handler;

        // ==========================================
        // SECTIONS (MUSCLE GROUPS)
        // ==========================================

        [HttpPost("sections")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(AdminSectionRes), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [EndpointSummary("Admin: Create a new predefined exercise section with EN & AR and image.")]
        public async Task<ActionResult> CreateSection([FromForm] AdminSectionReq req, CancellationToken ct = default)
        {
            var result = await _handler.CreateSection(req, ct);
            return result.Match(response => Ok(response), Problem);
        }

        [HttpPut("sections/{id:guid}")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(Result<Updated>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [EndpointSummary("Admin: Update a predefined section (EN/AR texts and image).")]
        public async Task<ActionResult> UpdateSection([FromRoute] Guid id, [FromForm] AdminSectionReq req, CancellationToken ct = default)
        {
            var result = await _handler.UpdateSection(id, req, ct);
            return result.Match(response => Ok(response), Problem);
        }

        [HttpDelete("sections/{id:guid}")]
        [ProducesResponseType(typeof(Result<Deleted>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [EndpointSummary("Admin: Delete a predefined section.")]
        public async Task<ActionResult> DeleteSection([FromRoute] Guid id, CancellationToken ct = default)
        {
            var result = await _handler.DeleteSection(id, ct);
            return result.Match(response => Ok(response), Problem);
        }

        [HttpGet("sections")]
        [ProducesResponseType(typeof(List<AdminSectionRes>), StatusCodes.Status200OK)]
        [EndpointSummary("Admin: Get all predefined sections with bilingual EN and AR values.")]
        public async Task<ActionResult> GetSections(CancellationToken ct = default)
        {
            var result = await _handler.GetAdminSections(ct);
            return result.Match(response => Ok(response), Problem);
        }

        // ==========================================
        // EXERCISES
        // ==========================================

        [HttpPost("exercises")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(AdminExerciseRes), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [EndpointSummary("Admin: Create a new predefined exercise with EN & AR fields and image.")]
        public async Task<ActionResult> CreateExercise([FromForm] AdminExerciseReq req, CancellationToken ct = default)
        {
            var result = await _handler.CreateExercise(req, ct);
            return result.Match(response => Ok(response), Problem);
        }

        [HttpPut("exercises/{id:guid}")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(Result<Updated>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [EndpointSummary("Admin: Update a predefined exercise (EN/AR texts, equipment, image).")]
        public async Task<ActionResult> UpdateExercise([FromRoute] Guid id, [FromForm] AdminExerciseReq req, CancellationToken ct = default)
        {
            var result = await _handler.UpdateExercise(id, req, ct);
            return result.Match(response => Ok(response), Problem);
        }

        [HttpDelete("exercises/{id:guid}")]
        [ProducesResponseType(typeof(Result<Deleted>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [EndpointSummary("Admin: Delete a predefined exercise.")]
        public async Task<ActionResult> DeleteExercise([FromRoute] Guid id, CancellationToken ct = default)
        {
            var result = await _handler.DeleteExercise(id, ct);
            return result.Match(response => Ok(response), Problem);
        }

        [HttpGet("exercises")]
        [ProducesResponseType(typeof(List<AdminExerciseRes>), StatusCodes.Status200OK)]
        [EndpointSummary("Admin: Get all predefined exercises with bilingual EN & AR details.")]
        public async Task<ActionResult> GetExercises([FromQuery] Guid? sectionId = null, CancellationToken ct = default)
        {
            var result = await _handler.GetAdminExercises(sectionId, ct);
            return result.Match(response => Ok(response), Problem);
        }

        // ==========================================
        // WORKDAYS (ROUTINE DAY SPLITS)
        // ==========================================

        [HttpPost("workdays")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(AdminWorkdayRes), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [EndpointSummary("Admin: Create a new predefined workday template with EN & AR and image.")]
        public async Task<ActionResult> CreateWorkday([FromForm] AdminWorkdayReq req, CancellationToken ct = default)
        {
            var result = await _handler.CreateWorkday(req, ct);
            return result.Match(response => Ok(response), Problem);
        }

        [HttpPut("workdays/{id:guid}")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(Result<Updated>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [EndpointSummary("Admin: Update a predefined workday template (EN/AR texts, image).")]
        public async Task<ActionResult> UpdateWorkday([FromRoute] Guid id, [FromForm] AdminWorkdayReq req, CancellationToken ct = default)
        {
            var result = await _handler.UpdateWorkday(id, req, ct);
            return result.Match(response => Ok(response), Problem);
        }

        [HttpDelete("workdays/{id:guid}")]
        [ProducesResponseType(typeof(Result<Deleted>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [EndpointSummary("Admin: Delete a predefined workday template.")]
        public async Task<ActionResult> DeleteWorkday([FromRoute] Guid id, CancellationToken ct = default)
        {
            var result = await _handler.DeleteWorkday(id, ct);
            return result.Match(response => Ok(response), Problem);
        }

        [HttpGet("workdays")]
        [ProducesResponseType(typeof(List<AdminWorkdayRes>), StatusCodes.Status200OK)]
        [EndpointSummary("Admin: Get all predefined workday templates with bilingual EN & AR details.")]
        public async Task<ActionResult> GetWorkdays(CancellationToken ct = default)
        {
            var result = await _handler.GetAdminWorkdays(ct);
            return result.Match(response => Ok(response), Problem);
        }

        // ==========================================
        // PREDEFINED WORKOUT SESSIONS (TEMPLATES)
        // ==========================================

        [HttpPost("sessions")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(AdminPredefinedSessionRes), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [EndpointSummary("Admin: Create a new predefined workout session template with exercises and image.")]
        public async Task<ActionResult> CreateSession([FromForm] AdminPredefinedSessionReq req, CancellationToken ct = default)
        {
            var result = await _handler.CreatePredefinedSession(req, ct);
            return result.Match(response => Ok(response), Problem);
        }

        [HttpPut("sessions/{id:guid}")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(Result<Updated>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [EndpointSummary("Admin: Update a predefined workout session template.")]
        public async Task<ActionResult> UpdateSession([FromRoute] Guid id, [FromForm] AdminPredefinedSessionReq req, CancellationToken ct = default)
        {
            var result = await _handler.UpdatePredefinedSession(id, req, ct);
            return result.Match(response => Ok(response), Problem);
        }

        [HttpDelete("sessions/{id:guid}")]
        [ProducesResponseType(typeof(Result<Deleted>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [EndpointSummary("Admin: Delete a predefined workout session template.")]
        public async Task<ActionResult> DeleteSession([FromRoute] Guid id, CancellationToken ct = default)
        {
            var result = await _handler.DeletePredefinedSession(id, ct);
            return result.Match(response => Ok(response), Problem);
        }

        [HttpGet("sessions")]
        [ProducesResponseType(typeof(List<AdminPredefinedSessionRes>), StatusCodes.Status200OK)]
        [EndpointSummary("Admin: Get all predefined workout session templates.")]
        public async Task<ActionResult> GetSessions([FromQuery] Guid? workdayId = null, CancellationToken ct = default)
        {
            var result = await _handler.GetAdminPredefinedSessions(workdayId, ct);
            return result.Match(response => Ok(response), Problem);
        }
    }
}
