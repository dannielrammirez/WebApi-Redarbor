using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Redarbor.Domain.Exceptions;

namespace Redarbor.API.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IHostEnvironment _environment;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger,
        IHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, problemDetails) = exception switch
        {
            DomainException domainEx => (
                HttpStatusCode.BadRequest,
                CreateProblemDetails("Error de dominio", domainEx.Message, (int)HttpStatusCode.BadRequest)),

            NotFoundException notFoundEx => (
                HttpStatusCode.NotFound,
                CreateProblemDetails("No encontrado", notFoundEx.Message, (int)HttpStatusCode.NotFound)),

            UnauthorizedAccessException => (
                HttpStatusCode.Unauthorized,
                CreateProblemDetails("No autorizado", "Acceso denegado.", (int)HttpStatusCode.Unauthorized)),

            ArgumentNullException argEx => (
                HttpStatusCode.BadRequest,
                CreateProblemDetails("Solicitud incorrecta", argEx.Message, (int)HttpStatusCode.BadRequest)),

            ArgumentException argEx => (
                HttpStatusCode.BadRequest,
                CreateProblemDetails("Solicitud incorrecta", argEx.Message, (int)HttpStatusCode.BadRequest)),

            OperationCanceledException => (
                HttpStatusCode.BadRequest,
                CreateProblemDetails("Solicitud cancelada", "La solicitud fue cancelada.", (int)HttpStatusCode.BadRequest)),

            _ => (
                HttpStatusCode.InternalServerError,
                CreateProblemDetails(
                    "Error interno del servidor",
                    _environment.IsDevelopment() ? exception.Message : "Ocurrió un error inesperado.",
                    (int)HttpStatusCode.InternalServerError))
        };

        _logger.LogError(
            exception,
            "Excepción ocurrida: {ExceptionType} - {Message}",
            exception.GetType().Name,
            exception.Message);

        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = (int)statusCode;

        var json = JsonSerializer.Serialize(problemDetails, JsonOptions);
        await context.Response.WriteAsync(json);
    }

    private static ProblemDetails CreateProblemDetails(string title, string detail, int status)
    {
        return new ProblemDetails
        {
            Title = title,
            Detail = detail,
            Status = status,
            Type = $"https://httpstatuses.com/{status}"
        };
    }
}

public static class ExceptionHandlingMiddlewareExtensions
{
    public static IApplicationBuilder UseExceptionHandling(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ExceptionHandlingMiddleware>();
    }
}
