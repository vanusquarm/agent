using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;

public interface IEventPublisher
{
    Task PublishAsync<TEvent>(TEvent @event, CancellationToken token = default) where TEvent : IEvent;
}

public interface IEvent : INotification
{
    //DefaultIdType Id { get; }
    DateTime CreationDate { get; }
    IDictionary<string, object> MetaData { get; }
}

public class EventPublisher : IEventPublisher
{
    private readonly IPublishEndpoint _publisher;
    private readonly ILogger<EventPublisher> _logger;

    public EventPublisher(IPublishEndpoint publisher, ILogger<EventPublisher> logger)
    {
        _publisher = publisher;
        _logger = logger;
    }

    public Task PublishAsync<TEvent>(TEvent @event, CancellationToken token = default) where TEvent : IEvent
    {
        return _publisher.Publish(@event, token);
    }
}