using System.Reflection;
using FluentValidation;
using FluentValidation.Results;
using ValidationResult = ATLAS.Kernel.Domain.Result.ValidationResult;

namespace ATLAS.Kernel.Infrastructure.Behaviors;

/// <summary>
/// MediatR pipeline behavior that executes all registered
/// <see cref="IValidator{T}"/> instances for the incoming request
/// before the handler is invoked.
/// </summary>
/// <remarks>
/// <para>
/// When validation fails, the pipeline short-circuits and returns a
/// <see cref="Result"/> or <see cref="Result{T}"/> failure without
/// invoking the handler.
/// </para>
/// <para>
/// Requires FluentValidation validators to be registered in the DI container,
/// typically via <c>services.AddValidatorsFromAssembly(assembly)</c>.
/// </para>
/// </remarks>
/// <typeparam name="TRequest">The request type.</typeparam>
/// <typeparam name="TResponse">
/// The response type. Must be <c>Result</c> or <c>Result&lt;T&gt;</c>
/// for the behavior to intercept failures gracefully.
/// </typeparam>
/// <example>
/// <code>
/// // Module DI registration:
/// services.AddTransient(typeof(IPipelineBehavior&lt;,&gt;), typeof(ValidationBehavior&lt;,&gt;));
/// services.AddValidatorsFromAssembly(typeof(CreateCustomerCommand).Assembly);
///
/// // Validator definition:
/// public sealed class CreateCustomerCommandValidator
///     : AbstractValidator&lt;CreateCustomerCommand&gt;
/// {
///     public CreateCustomerCommandValidator()
///     {
///         RuleFor(c => c.Name).NotEmpty().MaximumLength(200);
///         RuleFor(c => c.Email).NotEmpty().EmailAddress();
///     }
/// }
/// </code>
/// </example>
public sealed class ValidationBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    /// <summary>Initialises the behavior with all registered validators for <typeparamref name="TRequest"/>.</summary>
    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        => _validators = validators;

    /// <inheritdoc/>
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!_validators.Any()) return await next();

        var ctx = new ValidationContext<TRequest>(request);
        FluentValidation.Results.ValidationResult[] results = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(ctx, cancellationToken)));
        List<ValidationFailure> failures = results.SelectMany(r => r.Errors).Where(f => f is not null).ToList();

        if (failures.Count == 0)
            return await next(cancellationToken);

        var errors = failures
            .Select(f => new ValidationError(f.PropertyName, f.ErrorMessage, f.ErrorCode, f.AttemptedValue))
            .ToList();

        ValidationResult validationResult = ValidationResult.WithErrors(errors);

        if (typeof(TResponse) == typeof(Result))
            return (TResponse)(object)validationResult.ToResult();

        if (!typeof(TResponse).IsGenericType ||
            typeof(TResponse).GetGenericTypeDefinition() != typeof(Result<>))
        {
            throw new ValidationException(failures);
        }

        Error error = Error.Validation("Validation.Failed",
            $"Validation failed with {failures.Count} error(s).");
        MethodInfo failMethod = typeof(Result<>)
            .MakeGenericType(typeof(TResponse).GetGenericArguments()[0])
            .GetMethod(nameof(Result<object>.Fail), [typeof(Error)])!;
        return (TResponse)failMethod.Invoke(null, [error])!;

    }
}
