using GoodHamburger.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace GoodHamburger.Api.Middleware;

public class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (DuplicateItemException Exception)
        {
            logger.LogWarning(Exception, "Duplicate item error");
            await WriteProblemAsync(context, StatusCodes.Status409Conflict, "Duplicate item",
                Exception.Message);
        }
        catch (NotFoundException Exception)
        {
            logger.LogWarning(Exception, "Order not found");

            await WriteProblemAsync(
                context,
                StatusCodes.Status404NotFound,
                "Order not found",
                Exception.Message);
        }
        catch (DomainException Exception)
        {
            logger.LogWarning(Exception, "Domain rule violation");
            await WriteProblemAsync(context, StatusCodes.Status422UnprocessableEntity, "Business rule violated",
                Exception.Message);
        }
        catch (Exception Ex)
        {
            logger.LogError(Ex, "Unexpected error");
            await WriteProblemAsync(context, StatusCodes.Status500InternalServerError, "Erro interno", "Ocorreu um erro inesperado.");
        }
    }

    private static async Task WriteProblemAsync(HttpContext context, int status, string title, string detail)
    {
        context.Response.StatusCode = status;
        context.Response.ContentType = "application/problem+json";

        var Problem = new ProblemDetails()
        {
            Status = status,
            Title = title,
            Detail = detail
        };
        
        Problem.Extensions["traceId"] = context.TraceIdentifier;

        await context.Response.WriteAsJsonAsync(Problem);
    }
}