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
        catch (DomainException Exception)
        {
            logger.LogWarning(Exception, "Domain rule violation");
            await WriteProblemAsync(context, StatusCodes.Status422UnprocessableEntity, "Regra de negócio violada", Exception.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error");
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

        await context.Response.WriteAsJsonAsync(Problem);
    }
}