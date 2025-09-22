using GymAssistant_API.Model.Identity.Dtos;
using GymAssistant_API.Model.Results;
using GymAssistant_API.Req_Res.Reqeust;

namespace GymAssistant_API.Repository.Interfaces.User
{
    public interface IUserCreate
    {
        Task<Result<AppUserDto>> AddUserAsync(RegisterRequest request, CancellationToken ct = default);
    }
}
