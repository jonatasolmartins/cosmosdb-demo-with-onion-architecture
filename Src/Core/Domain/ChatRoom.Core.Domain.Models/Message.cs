using Newtonsoft.Json;

namespace ChatRoom.Core.Domain.Models;

public class Message
{
    [JsonProperty(propertyName:"id")]
    public Guid Id { get; set; }
    public Guid RoomId { get; set; }
    public Guid ChatId { get; set; }

    [JsonProperty("partitionKey")] public string PartitionKey { get; set; }
    
    [JsonProperty(PropertyName = "dateCreated")]
    public DateTime DateCreated { get; set; }

    public string Content { get; set; }
    public User User { get; set; }
}