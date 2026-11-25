using Microsoft.AspNetCore.Diagnostics;

namespace ExchangeMarket.Api.Exceptions;

public class AppExceptionHandler(IProblemDetailsService problemDetails) : IExceptionHandler
{
public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken ct)
    {
        if (exception is not AppExceptions appEx) return false;

        context.Response.StatusCode = appEx.StatusCode;
        return await problemDetails.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = context,
            Exception = exception,
            ProblemDetails = { Status = appEx.StatusCode, Title = appEx.Message }
        });
    }
}