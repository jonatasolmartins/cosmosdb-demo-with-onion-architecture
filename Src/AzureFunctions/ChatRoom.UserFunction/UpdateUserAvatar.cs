using System.Threading.Tasks;
using System;
using ChatRoom.Core.Domain.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ChatRoom.UserFunction;

public class UpdateUserAvatar
{

    [FunctionName("UpdateUserAvatar")]
    public async Task RunAsync([QueueTrigger("update-avatar", Connection = "QueueConnection")] string myQueueItem, ILogger log)
    {
        try
        {
            if (myQueueItem != null)
            {
                var user = JsonConvert.DeserializeObject<User>(myQueueItem);
                //TODO: Update the user document inside message container
                // As we keep only the first 3 message in the message array in room container we don't need to update that,
                // it will soon be delete from container to give place to a new message.
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        
    }
}