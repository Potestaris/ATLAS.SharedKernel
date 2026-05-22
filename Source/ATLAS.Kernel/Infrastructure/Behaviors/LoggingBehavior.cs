namespace ATLAS.Kernel.Infrastructure.Behaviors;

/// <summary>
/// MediatR pipeline behavior that logs the start, completion, and elapsed time of
/// every <see cref="IRequest{TResponse}"/>. Emits a warning for requests that
/// exceed <see cref="SlowRequestThresholdMs"/> milliseconds.
/// </summary>
/// <remarks>
/// Register <b>first</b> in the pipeline so it measures total execution time
/// including all subsequent behaviors.
/// </remarks>
/// <typeparam name="TRequest">The request type.</typeparam>
/// <typeparam name="TResponse">The response type.</typeparam>
/// <example>
/// <code>
/// // Module DI registration:
/// services.AddTransient(typeof(IPipelineBehavior&lt;,&gt;), typeof(LoggingBehavior&lt;,&gt;));
///
/// // Adjust the slow-request threshold globally:
/// LoggingBehavior&lt;,&gt;.SlowRequestThresholdMs = 1000;
/// </code>
/// </example>
public sealed class LoggingBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    // ReSharper disable StaticMemberInGenericType
    /// <summary>
    /// Requests exceeding this duration (in milliseconds) are logged as warnings.
    /// Default: 500 ms.
    /// </summary>
    public static int SlowRequestThresholdMs { get; set; } = 500;
    // ReSharper restore StaticMemberInGenericType

    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    /// <summary>Initialises the behavior with the provided logger.</summary>
    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
        => _logger = logger;

    /// <inheritdoc/>
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        string name = typeof(TRequest).Name;
        _logger.LogInformation("[Pipeline] Handling {RequestName}", name);

        var sw = Stopwatch.StartNew();
        TResponse response;
        try
        {
            response = await next();
        }
        catch (Exception ex)
        {
            sw.Stop();
            _logger.LogError(ex, "[Pipeline] {RequestName} failed after {ElapsedMs}ms",
                name, sw.ElapsedMilliseconds);
            throw;
        }

        sw.Stop();

        if (sw.ElapsedMilliseconds > SlowRequestThresholdMs)
            _logger.LogWarning(
                "[Pipeline] Slow request — {RequestName} took {ElapsedMs}ms (threshold: {ThresholdMs}ms)",
                name, sw.ElapsedMilliseconds, SlowRequestThresholdMs);
        else
            _logger.LogInformation("[Pipeline] {RequestName} completed in {ElapsedMs}ms",
                name, sw.ElapsedMilliseconds);

        return response;
    }
}
