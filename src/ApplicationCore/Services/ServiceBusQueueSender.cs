
using System;
using Azure.Identity;
using Azure.Messaging.ServiceBus;

namespace Microsoft.eShopWeb.Web.Services;

public interface IServiceBusQueueSender
{
    void Send(string message);
}

public class ServiceBusQueueSender : IDisposable, IServiceBusQueueSender
{
    ServiceBusSender _sender;
    ServiceBusClient _client;

    public ServiceBusQueueSender()
    {
        var clientOptions = new ServiceBusClientOptions
        { 
            TransportType = ServiceBusTransportType.AmqpWebSockets
        };
        _client = new ServiceBusClient(
            "eshoponwebservicebusbarpod.servicebus.windows.net",
            new DefaultAzureCredential(),
            clientOptions);
        _sender = _client.CreateSender("orders");
    }

    public async void Send(string message)
    {
        using ServiceBusMessageBatch messageBatch = await _sender.CreateMessageBatchAsync();
        if (!messageBatch.TryAddMessage(new ServiceBusMessage(message)))
        {
            throw new Exception($"The message  is too large to fit in the batch.");
        }
        await _sender.SendMessagesAsync(messageBatch);
    }

    public async void Dispose()
    {
        await _sender.DisposeAsync();
        await _client.DisposeAsync();
    }
}
