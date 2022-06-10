using Azure.Storage.Queues;
using ChatRoom.Core.Domain.Models;
using Newtonsoft.Json;

namespace ChatRoom.Core.Application.Services.BackgroundService;

public class MessageProcessor: Microsoft.Extensions.Hosting.BackgroundService
{
    private readonly QueueClient _queueClient;
    
    public MessageProcessor(QueueClient queueClient)
    {
        _queueClient = queueClient;
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
                    //TODO: Update the user document inside message container
                    // As we keep only the first 3 message in the message array in room container we don't need to update that,
                    // it will soon be delete from container to give place to a new message.
                    await _queueClient.DeleteMessageAsync(message.Value.MessageId, message.Value.PopReceipt, stoppingToken);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            
            await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
        }
        
    }
}