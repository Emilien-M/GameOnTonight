using FluentValidation;
using FluentValidation.Results;
using GameOnTonight.Domain.Exceptions;
using Mediator;

namespace GameOnTonight.Application.Behaviors;

/// <summary>
/// Validation behavior for the Mediator pipeline.
/// Validates commands and queries using FluentValidation.
/// </summary>
/// <typeparam name="TRequest">Type of the request.</typeparam>
/// <typeparam name="TResponse">Type of the response.</typeparam>
public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull, IMessage
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async ValueTask<TResponse> Handle(
        TRequest message,
        MessageHandlerDelegate<TRequest, TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any())
        {
            return await next(message, cancellationToken);
        }

        var context = new ValidationContext<TRequest>(message);
        var validationResults = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        var failures = validationResults
            .SelectMany(r => r.Errors)
            .Where(f => f != null)
            .ToList();

        if (failures.Count != 0)
        {
            ThrowValidationException(failures);
        }

        return await next(message, cancellationToken);
    }

    private static void ThrowValidationException(List<ValidationFailure> failures)
    {
        throw new DomainException(failures.Select(x => (x.PropertyName, x.ErrorMessage)));
    }
}
