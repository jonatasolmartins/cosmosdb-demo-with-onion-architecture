using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ChatRoom.Core.Domain.Abstractions.Repositories;
using ChatRoom.Core.Domain.Models;
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ChatRoom.Infrastructure.AzureFunction;

public class UpdateMessageDocument
{
    private readonly IRepository _roomRepository;

    public UpdateMessageDocument(IRepository roomRepository)
    {
        _roomRepository = roomRepository;
    }

    [FunctionName("UpdateMessageDocument")]
    public async Task RunAsync([CosmosDBTrigger(
            databaseName: "BootCampDemo",
            collectionName: "Message",
            ConnectionStringSetting = "DBConnection",
            LeaseCollectionName = "leases",
            CreateLeaseCollectionIfNotExists = true)]
        IReadOnlyList<Document> input, ILogger log)
    {
        try
        {
            if (input is {Count: > 0})
            {
                foreach (var item in input)
                {
                    var message = JsonConvert.DeserializeObject<Message>(item.ToString());
                    await _roomRepository.UpdateRecentMessage(message);
                    log.LogInformation($"Room messages updated {input.Count}");
                }
            
            }
        }
        catch (Exception ex)
        {
            log.LogError(ex.Message);
            throw;
        }
    }
}