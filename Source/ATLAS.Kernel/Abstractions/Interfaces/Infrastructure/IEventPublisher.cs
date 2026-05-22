namespace ATLAS.Kernel.Abstractions.Interfaces.Infrastructure;

/// <summary>
/// Provides the contract for publishing integration events to the message bus
/// (RabbitMQ or Azure Service Bus) for cross-module, asynchronous communication.
/// </summary>
/// <remarks>
/// Integration events should be published <b>after</b> the unit-of-work has
/// successfully committed to ensure consistency. Domain events (in-process)
/// are dispatched via MediatR's <c>IPublisher</c>, not via this interface.
/// </remarks>
/// <example>
/// <code>
/// // Publishing after successful save in a command handler:
/// await unitOfWork.SaveChangesAsync(ct);
/// await eventPublisher.PublishAsync(
///     new CustomerCreatedIntegrationEvent(customer.Id, customer.Code, tenantId), ct);
/// </code>
/// </example>
public interface IEventPublisher
{
    /// <summary>
    /// Publishes a strongly-typed integration event to the configured message bus.
    /// </summary>
    /// <typeparam name="T">The concrete integration event type.</typeparam>
    /// <param name="integrationEvent">The event to publish. Must not be null.</param>
    /// <param name="cancellationToken">Propagates notification that the operation should be cancelled.</param>
    Task PublishAsync<T>(T integrationEvent, CancellationToken cancellationToken = default) where T : class, IIntegrationEvent;

    /// <summary>
    /// Publishes an integration event using the base interface, useful when the
    /// concrete type is not known at compile time.
    /// </summary>
    Task PublishAsync(IIntegrationEvent integrationEvent, CancellationToken cancellationToken = default);
}
