namespace ATLAS.Kernel.Infrastructure.Behaviors;

/// <summary>
/// MediatR pipeline behavior that caches responses for requests that implement
/// <see cref="ICacheableRequest"/>. The cache is skipped entirely for requests
/// that do not implement this interface.
/// </summary>
/// <remarks>
/// <para>
/// Register this behavior <b>only</b> on query pipelines.
/// Commands must never be cached — they produce side effects.
/// </para>
/// <para>
/// The cache key and TTL are supplied by the request itself via
/// <see cref="ICacheableRequest.CacheKey"/> and
/// <see cref="ICacheableRequest.CacheDuration"/>.
/// </para>
/// </remarks>
/// <typeparam name="TRequest">The request type. Caching activates only when it implements <see cref="ICacheableRequest"/>.</typeparam>
/// <typeparam name="TResponse">The response type.</typeparam>
/// <example>
/// <code>
/// // Making a query cacheable:
/// public sealed record GetActiveCountriesQuery()
///     : IRequest&lt;Result&lt;IReadOnlyList&lt;CountryDto&gt;&gt;&gt;, ICacheableRequest
/// {
///     public string CacheKey      => "master:countries:active";
///     public TimeSpan? CacheDuration => TimeSpan.FromHours(1);
/// }
///
/// // Registration:
/// services.AddTransient(typeof(IPipelineBehavior&lt;,&gt;), typeof(CachingBehavior&lt;,&gt;));
/// </code>
/// </example>
public sealed class CachingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    private readonly ICacheService _cache;
    private readonly ILogger<CachingBehavior<TRequest, TResponse>> _logger;

    /// <summary>Initializes the behavior with the required cache service and logger.</summary>
    public CachingBehavior(ICacheService cache, ILogger<CachingBehavior<TRequest, TResponse>> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        // ReSharper disable once SuspiciousTypeConversion.Global
        if (request is not ICacheableRequest cacheable)
            return await next(cancellationToken);

        var cached = await _cache.GetAsync<TResponse>(cacheable.CacheKey, cancellationToken);
        if (cached is not null)
        {
            _logger.LogDebug("[Cache] HIT  — key: {CacheKey}", cacheable.CacheKey);
            return cached;
        }

        _logger.LogDebug("[Cache] MISS — key: {CacheKey}", cacheable.CacheKey);
        TResponse response = await next(cancellationToken);

        await _cache.SetAsync(cacheable.CacheKey, response, cacheable.CacheDuration, cancellationToken);
        return response;
    }
}
