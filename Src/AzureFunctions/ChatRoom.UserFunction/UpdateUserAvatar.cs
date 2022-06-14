using System;
using System.Threading.Tasks;
using ChatRoom.Core.Domain.Abstractions.Services;
using ChatRoom.Core.Domain.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ChatRoom.UserFunction;

public class UpdateUserAvatar
{
    private readonly IMessageService _messageService;

    public UpdateUserAvatar(IMessageService messageService)
    {
        _messageService = messageService;
    }

    [FunctionName("UpdateUserAvatar")]
    public async Task RunAsync([QueueTrigger("update-avatar", Connection = "QueueConnection")] string myQueueItem, ILogger log)
    {
        try
        {
            if (myQueueItem != null)
            {
                var user = JsonConvert.DeserializeObject<User>(myQueueItem);
                await _messageService.UpdateMessageUserAvatar(user);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        
    }
}