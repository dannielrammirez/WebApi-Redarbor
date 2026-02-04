using FluentValidation;
using MediatR;
using Redarbor.Application.Common;

namespace Redarbor.Application.Behaviors;

public sealed class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : Result
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any())
            return await next();

        var context = new ValidationContext<TRequest>(request);

        var validationResults = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        var failures = validationResults
            .SelectMany(r => r.Errors)
            .Where(f => f != null)
            .ToList();

        if (failures.Count != 0)
        {
            var errors = failures.Select(f => f.ErrorMessage).ToList();
            return CreateValidationResult<TResponse>(errors);
        }

        return await next();
    }

    private static TResponse CreateValidationResult<T>(IReadOnlyList<string> errors)
        where T : Result
    {
        if (typeof(T) == typeof(Result))
        {
            return (Result.Failure(errors) as TResponse)!;
        }

        var resultType = typeof(T).GetGenericArguments()[0];
        var failureMethod = typeof(Result).GetMethod(nameof(Result.Failure), 1, new[] { typeof(IReadOnlyList<string>) });
        var genericFailureMethod = failureMethod!.MakeGenericMethod(resultType);
        return (TResponse)genericFailureMethod.Invoke(null, new object[] { errors })!;
    }
}
