using GymAssistant_API.Model.Results;
using Microsoft.AspNetCore.Identity;

namespace GymAssistant_API.Extensions
{
    public class IdentityServiceExtensions
    {
        public static List<Error> ConvertIdentityErrors(IEnumerable<IdentityError> identityErrors)
        {
            return identityErrors.Select(error => Error.Validation(error.Code, error.Description)).ToList();
        }
    }
}
