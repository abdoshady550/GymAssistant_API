using GymAssistant_API.Model.Results;
using GymAssistant_API.Repository.Interfaces.User;
using GymAssistant_API.Req_Res.Reqeust;
using GymAssistant_API.Req_Res.Response;
using System.Security.Claims;

namespace GymAssistant_API.Handeler.User
{
    public sealed class CreateProfileHandler(ILogger<CreateProfileHandler> logger,
                                      IProfile profile)
    {
        private readonly ILogger<CreateProfileHandler> _logger = logger;
        private readonly IProfile _profile = profile;
        public async Task<Result<ProfileResponse>> Handle(string Id,
                                                          CreateProfileRequest request,
                                                          MeasurementRequest measurementRequest)
        {
            var createProfile = await _profile
                .CreateProfileAsync(Id, request.FirstName, request.LastName, request.Gender);
            if (createProfile.IsError)
            {
                _logger.LogError("Failed to create profile for user {UserId}: {Error}", Id, createProfile.Errors);
                return createProfile.Errors;

            }
            var addMeasurements = await _profile
                .AddBodyMeasurementAsync(Id,
                                         measurementRequest.WeightKg,
                                         measurementRequest.MuscleMassKg,
                                         measurementRequest.BodyFatPercent);
            if (addMeasurements.IsError)
            {
                _logger.LogError("Failed to add measurements for user {UserId}: {Error}", Id, addMeasurements.Errors);
                return addMeasurements.Errors;
            }
            var profile = createProfile.Value;
            var measurements = addMeasurements.Value;
            var response = new ProfileResponse
            {
                FirstName = profile.FirstName,
                LastName = profile.LastName,
                Gender = profile.Gender,
                Role = profile.Role,
                WeightKg = measurements.WeightKg,
                MuscleMassKgdecimal = measurements.MuscleMassKg,
                BodyFatPercent = measurements.BodyFatPercent

            };
            return response;
        }


    }
}
