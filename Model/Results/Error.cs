
namespace GymAssistant_API.Model.Results;

public readonly record struct Error(string Code, string Description, ErrorKind Type, object[]? Args = null)
{
    public static Error None => new(string.Empty, string.Empty, ErrorKind.None);

    public static Error Failure(string code, string description, params object[] args) =>
        new(code, description, ErrorKind.Failure, args.Length > 0 ? args : null);

    public static Error Failure(string code, params object[] args) =>
        new(code, code, ErrorKind.Failure, args.Length > 0 ? args : null);

    public static Error Unexpected(string code, string description, params object[] args) =>
        new(code, description, ErrorKind.Unexpected, args.Length > 0 ? args : null);

    public static Error Unexpected(string code, params object[] args) =>
        new(code, code, ErrorKind.Unexpected, args.Length > 0 ? args : null);

    public static Error Validation(string code, string description, params object[] args) =>
        new(code, description, ErrorKind.Validation, args.Length > 0 ? args : null);

    public static Error Validation(string code, params object[] args) =>
        new(code, code, ErrorKind.Validation, args.Length > 0 ? args : null);

    public static Error Conflict(string code, string description, params object[] args) =>
        new(code, description, ErrorKind.Conflict, args.Length > 0 ? args : null);

    public static Error Conflict(string code, params object[] args) =>
        new(code, code, ErrorKind.Conflict, args.Length > 0 ? args : null);

    public static Error NotFound(string code, string description, params object[] args) =>
        new(code, description, ErrorKind.NotFound, args.Length > 0 ? args : null);

    public static Error NotFound(string code, params object[] args) =>
        new(code, code, ErrorKind.NotFound, args.Length > 0 ? args : null);

    public static Error Unauthorized(string code, string description, params object[] args) =>
        new(code, description, ErrorKind.Unauthorized, args.Length > 0 ? args : null);

    public static Error Unauthorized(string code, params object[] args) =>
        new(code, code, ErrorKind.Unauthorized, args.Length > 0 ? args : null);
}