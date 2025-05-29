using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HabitsApp.Domain.Shared;
public sealed class Result<T>
{
    public T? Value { get; set; }
    public bool IsSuccess { get; set; }
    public bool IsFailure => !IsSuccess;
    public string? Message { get; set; }
    public string[]? ErrorMessages { get; set; }

    public Result(T? value, bool ısSuccess, string[]? errorMessages, string? message)
    {
        Value = value;
        IsSuccess = ısSuccess;
        ErrorMessages = errorMessages;
        Message = message;
    }

    public static Result<T> Success(T? value,string? message)
    {
        return new Result<T>(value, true, null,message);
    }
    public static Result<T> Failure(params string[]? errorMessages)
    {
        return new Result<T>(default, false, errorMessages, null);
    }
}
