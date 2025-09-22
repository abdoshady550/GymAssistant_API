using Azure.Core;
using GymAssistant_API.Data;
using GymAssistant_API.Model.Entities.User;
using GymAssistant_API.Model.Identity.Dtos;
using GymAssistant_API.Model.Results;
using GymAssistant_API.Repository.Interfaces.User;
using GymAssistant_API.Req_Res.Reqeust;
using Microsoft.AspNetCore.Identity;

namespace GymAssistant_API.Handeler.Identity
{
    public class RegisterHandler(
    ILogger<ApplicationDbContextInitialiser> logger,
   IUserCreate userCreate)
    {
        private readonly ILogger<ApplicationDbContextInitialiser> _logger = logger;
        private readonly IUserCreate _userCreate = userCreate;


        public async Task<Result<AppUserDto>> Handle(RegisterRequest Request, CancellationToken ct)
        {
            var createUser = await _userCreate.AddUserAsync(Request, ct);

            if (createUser.IsError)
            {
                _logger.LogError("User with UserName { UserName }{ErrorDetails}", Request.UserName, createUser.TopError.Description);
                return createUser.Errors;
            }

            return createUser.Value;




        }




    }
}

