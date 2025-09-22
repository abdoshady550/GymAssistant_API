using GymAssistant_API.Data;
using GymAssistant_API.Model.Results;
using GymAssistant_API.Repository.Interfaces.User;
using GymAssistant_API.Req_Res.Reqeust;
using GymAssistant_API.Req_Res.Response;
using Microsoft.EntityFrameworkCore;

namespace GymAssistant_API.Handeler.User
{
    public sealed class UpdateProfileHandler(ILogger<UpdateProfileHandler> logger,
                                             IProfile profile,
                                             AppDbContext context)
    {
        private readonly ILogger<UpdateProfileHandler> _logger = logger;
        private readonly IProfile _profile = profile;
        private readonly AppDbContext _context = context;

        public async Task<Result<Updated>> Handle(string id, UpdateProfileRequest request, CancellationToken ct = default)
        {
            var user = await _context.Users.FindAsync(id, ct);
            if (user == null)
            {
                _logger.LogError("User not found with ID {UserId}", id);
                return Error.NotFound("User_NotFound", "User not found.");
            }
            var updateProfile = await _context.ClientProfiles
                .FirstOrDefaultAsync(p => p.AppUserId == user.Id);


            var result = await _profile
                .UpdateProfileAsync(updateProfile.Id,
                                    request.FirstName,
                                    request.LastName,
                                    request.Gender,
                                    request.BirthDate,
                                    request.HeightCm, ct);
            if (result.IsError)
            {
                _logger.LogError("Failed to update profile for user: {profileId}: {Error}", id, result.Errors);
                return result.Errors;

            }


            var updateMeasurements = await _context.BodyMeasurements
                .FirstOrDefaultAsync(m => m.ClientProfileId == updateProfile.Id, ct);
            if (updateMeasurements == null)
            {
                _logger.LogError("No measurements found for profile {ProfileId}", id);
                return Error.NotFound("Measurements_NotFound", "No measurements found for profile");
            }
            var measurementsResult = await _profile.UpdateBodyMeasurementAsync(updateMeasurements.Id,
                                                                         request.WeightKg,
                                                                         request.BodyFatPercent,
                                                                         request.MuscleMassKg, ct);
            if (measurementsResult.IsError)
            {
                _logger.LogError("Failed to update measurements for user: {profileId}: {Error}", id, measurementsResult.Errors);
                return measurementsResult.Errors;

            }
            return Result.Updated;

        }
    }
}
