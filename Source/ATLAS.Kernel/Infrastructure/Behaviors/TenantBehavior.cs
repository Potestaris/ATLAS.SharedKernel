using System.Reflection;

namespace ATLAS.Kernel.Infrastructure.Behaviors;

/// <summary>
/// MediatR pipeline behavior that validates the resolved tenant context before
/// any handler executes. Ensures that every request is processed within a valid,
/// active tenant scope.
/// </summary>
/// <remarks>
/// <para>
/// The tenant is resolved by <c>TenantMiddleware</c> before the request enters
/// the MediatR pipeline. This behavior acts as a second line of defence, rejecting
/// any request that reached the pipeline without a valid tenant context.
/// </para>
/// <para>
/// Background jobs and system-level operations that legitimately run outside a
/// tenant context should use a dedicated <c>SystemTenantContext</c> implementation
/// that returns <see cref="ITenantContext.IsResolved"/> = <c>true</c> with a
/// well-known system tenant ID.
/// </para>
/// </remarks>
/// <typeparam name="TRequest">The request type.</typeparam>
/// <typeparam name="TResponse">The response type.</typeparam>
/// <example>
/// <code>
/// // Module DI registration:
/// services.AddTransient(typeof(IPipelineBehavior&lt;,&gt;), typeof(TenantBehavior&lt;,&gt;));
/// </code>
/// </example>
public sealed class TenantBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    private readonly ITenantContext _tenantContext;
    private readonly ILogger<TenantBehavior<TRequest, TResponse>> _logger;

    /// <summary>Initialises the behavior with the tenant context and logger.</summary>
    public TenantBehavior(ITenantContext tenantContext, ILogger<TenantBehavior<TRequest, TResponse>> logger)
    {
        _tenantContext = tenantContext;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!_tenantContext.IsResolved)
        {
            _logger.LogWarning(
                "[TenantBehavior] Request {RequestName} rejected — tenant context not resolved.",
                typeof(TRequest).Name);

            Error error = Error.Unauthorized("Tenant.NotResolved",
                "The request could not be processed because the tenant identity was not established.");

            if (typeof(TResponse) == typeof(Result))
                return (TResponse)(object)Result.Fail(error);

            if (!typeof(TResponse).IsGenericType ||
                typeof(TResponse).GetGenericTypeDefinition() != typeof(Result<>))
            {
                throw new UnauthorizedAccessException(error.Message);
            }

            MethodInfo failMethod = typeof(Result<>)
                .MakeGenericType(typeof(TResponse).GetGenericArguments()[0])
                .GetMethod(nameof(Result<object>.Fail), [typeof(Error)])!;
            return (TResponse)failMethod.Invoke(null, [error])!;

        }

        _logger.LogDebug("[TenantBehavior] Tenant={TenantId} — handling {RequestName}",
            _tenantContext.TenantId, typeof(TRequest).Name);

        return await next();
    }
}
