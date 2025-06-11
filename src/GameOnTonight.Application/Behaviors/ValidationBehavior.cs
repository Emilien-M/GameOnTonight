using FluentValidation;
using FluentValidation.Results;
using GameOnTonight.Domain.Exceptions;
using Mediator;

namespace GameOnTonight.Application.Behaviors;

/// <summary>
/// Comportement de validation pour le pipeline Mediator
/// Valide les commandes et requêtes via FluentValidation
/// </summary>
/// <typeparam name="TRequest">Type de la requête</typeparam>
/// <typeparam name="TResponse">Type de la réponse</typeparam>
public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async ValueTask<TResponse> Handle(
        TRequest request, 
        CancellationToken cancellationToken, 
        MessageHandlerDelegate<TRequest, TResponse> next)
    {
        if (!_validators.Any())
        {
            return await next(request, cancellationToken);
        }

        var context = new ValidationContext<TRequest>(request);
        var validationResults = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        var failures = validationResults
            .SelectMany(r => r.Errors)
            .Where(f => f != null)
            .ToList();

        if (failures.Any())
        {
            ThrowValidationException(failures);
        }

        return await next(request, cancellationToken);
    }

    private static void ThrowValidationException(List<ValidationFailure> failures)
    {
        var errors = failures
            .GroupBy(e => e.PropertyName)
            .ToDictionary(
                g => g.Key, 
                g => g.Select(e => e.ErrorMessage).ToArray()
            );

        throw new DomainException("Des erreurs de validation sont survenues", errors);
    }
}
