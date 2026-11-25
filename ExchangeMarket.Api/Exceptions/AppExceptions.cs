namespace ExchangeMarket.Api.Exceptions;

public abstract class AppExceptions(string message, int statusCode) : Exception(message)
{
    public int StatusCode { get; } = statusCode;
}

public class NotFoundException(string message) 
    : AppExceptions(message, StatusCodes.Status404NotFound);
public class BadRequestException(string message) 
    : AppExceptions(message, StatusCodes.Status400BadRequest);
public class ForbiddenException (string message) 
    : AppExceptions(message, StatusCodes.Status403Forbidden);
public class ConflictException(string message) 
    : AppExceptions(message, StatusCodes.Status409Conflict);
