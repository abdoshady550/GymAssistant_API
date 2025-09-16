using System.Security.Claims;
using GymAssistant_API.Model.Identity.Dtos;
using GymAssistant_API.Model.Results;

namespace GymAssistant_API.Repository.Interfaces.Identity;

public interface ITokenProvider
{
    Task<Result<TokenResponse>> GenerateJwtTokenAsync(AppUserDto user, CancellationToken ct = default);

    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
}