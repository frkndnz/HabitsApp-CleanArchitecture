
using FluentValidation;
using HabitsApp.Domain.Shared;
using Microsoft.AspNetCore.Diagnostics;

namespace HabitsApp.WebAPI;

public  class ExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        Result<string> errorResult;


        httpContext.Response.ContentType = "application/json";
        httpContext.Response.StatusCode = 500;

        if(exception.GetType() == typeof(ValidationException))
        {
            httpContext.Response.StatusCode = 400;
            ValidationException validationException = (ValidationException)exception;
            var dict = validationException.Errors.ToDictionary(e => e.PropertyName, e => e.ErrorMessage).ToArray();
            var list = validationException.Errors.Select(e => $"{e.PropertyName} : {e.ErrorMessage}").ToArray();
            errorResult = Result<string>.Failure(list);   
            

            await httpContext.Response.WriteAsJsonAsync(errorResult);
            return true;
        }
        errorResult = Result<string>.Failure(exception.Message);
        await httpContext.Response.WriteAsJsonAsync(errorResult);


        return true;
    }
}
