using System.Collections.Generic;
using System.Linq;

namespace Binho.Core.Contracts.Shared;

public sealed record Result(bool IsSuccess, IReadOnlyList<string> Errors)
{
    public static Result Success() => new(true, []);
    public static Result Failure(params string[] errors) => new(false, errors);
    public static Result Failure(IEnumerable<string> errors) => new(false, errors.ToArray());
}

public sealed record Result<T>(bool IsSuccess, T? Value, IReadOnlyList<string> Errors)
{
    public static Result<T> Success(T value) => new(true, value, []);
    public static Result<T> Failure(params string[] errors) => new(false, default, errors);
    public static Result<T> Failure(IEnumerable<string> errors) => new(false, default, errors.ToArray());
}
