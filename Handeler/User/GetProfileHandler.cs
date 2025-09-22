using GymAssistant_API.Model.Results;
using GymAssistant_API.Repository.Interfaces.User;
using GymAssistant_API.Req_Res.Response;

namespace GymAssistant_API.Handeler.User
{
    public sealed class GetProfileHandler(ILogger<GetProfileHandler> logger,
                                   IProfile profile)
    {
        private readonly ILogger<GetProfileHandler> logger = logger;
        private readonly IProfile profile = profile;
        public async Task<Result<ProfileResponse>> Handle(string Id, CancellationToken ct = default)
        {
            var getProfile = await profile.GetProfileAsync(Id, ct);
            if (getProfile.IsError)
            {
                logger.LogError("Failed to get profile for user {UserId}: {Error}", Id, getProfile.Errors);
                return getProfile.Errors;
            }
            return getProfile;

        }
    }
}
