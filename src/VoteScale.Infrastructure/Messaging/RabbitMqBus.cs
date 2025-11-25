using MassTransit;
using VoteScale.Domain.Interfaces;

namespace VoteScale.Infrastructure.Messaging;

public class RabbitMQBus : IMessageBus
{
    private readonly IPublishEndpoint _publishEndpoint;

    public RabbitMQBus(IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }

    public async Task PublishAsync<T>(T message, CancellationToken cancellationToken = default) where T : class
    {
        await _publishEndpoint.Publish(message, cancellationToken);
    }
}