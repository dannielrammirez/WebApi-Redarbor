using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;
using Redarbor.Application.Common;

namespace Redarbor.Application.Behaviors;

public sealed class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : Result
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var requestId = Guid.NewGuid().ToString("N")[..8];

        _logger.LogInformation(
            "[{RequestId}] Procesando {RequestName}",
            requestId, requestName);

        var stopwatch = Stopwatch.StartNew();

        try
        {
            var response = await next();

            stopwatch.Stop();

            if (response.IsFailure)
            {
                _logger.LogWarning(
                    "[{RequestId}] {RequestName} completado con fallo en {ElapsedMs}ms: {Error}",
                    requestId, requestName, stopwatch.ElapsedMilliseconds, response.Error);
            }
            else
            {
                _logger.LogInformation(
                    "[{RequestId}] {RequestName} completado exitosamente en {ElapsedMs}ms",
                    requestId, requestName, stopwatch.ElapsedMilliseconds);
            }

            return response;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();

            _logger.LogError(
                ex,
                "[{RequestId}] {RequestName} fallido después de {ElapsedMs}ms",
                requestId, requestName, stopwatch.ElapsedMilliseconds);

            throw;
        }
    }
}
