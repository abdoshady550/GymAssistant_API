using GymAssistant_API.Model.Identity.Dtos;
using GymAssistant_API.Model.Results;
using GymAssistant_API.Repository.Interfaces.Identity;
using GymAssistant_API.Req_Res.Reqeust;

namespace GymAssistant_API.Handeler.Identity
{
    public class GetUserByIdQueryHanlder(ILogger<GetUserByIdQueryHanlder> logger,
                                         IIdentityService identityService)

    {
        private readonly ILogger<GetUserByIdQueryHanlder> _logger = logger;
        private readonly IIdentityService _identityService = identityService;
        public async Task<Result<AppUserDto>> Handle(GetUserByIdQuery request, CancellationToken ct)
        {
            var getUserByIdResult = await _identityService.GetUserByIdAsync(request.UserId!);

            if (getUserByIdResult.IsError)
            {
                _logger.LogError("User with Id { UserId }{ErrorDetails}", request.UserId, getUserByIdResult.TopError.Description);

                return getUserByIdResult.Errors;
            }

            return getUserByIdResult.Value;
        }
    }
}
