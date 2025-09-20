
namespace GymAssistant_API.Model.Results;

public enum ErrorKind
{
    None,
    Failure,
    Unexpected,
    Validation,
    Conflict,
    NotFound,
    Unauthorized
}
