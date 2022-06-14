using Azure.Storage.Queues;
using ChatRoom.Core.Domain.Abstractions.Services;
using ChatRoom.Core.Domain.Models;
using Newtonsoft.Json;

namespace ChatRoom.Core.Application.Services.BackgroundService;

public class MessageProcessor: Microsoft.Extensions.Hosting.BackgroundService
{
    private readonly QueueClient _queueClient;
    private readonly IMessageService _messageService;

    public MessageProcessor(QueueClient queueClient, IMessageService messageService)
    {
        _queueClient = queueClient;
        _messageService = messageService;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var message = await _queueClient.ReceiveMessageAsync(TimeSpan.FromSeconds(40), stoppingToken);
                if (message.Value != null)
                {
                    var user = JsonConvert.DeserializeObject<User>(message.Value.MessageText);
                    await _messageService.UpdateMessageUserAvatar(user);
                    await _queueClient.DeleteMessageAsync(message.Value.MessageId, message.Value.PopReceipt, stoppingToken);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }
        
    }
}