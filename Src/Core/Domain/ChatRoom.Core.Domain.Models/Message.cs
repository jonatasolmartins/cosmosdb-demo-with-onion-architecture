using Newtonsoft.Json;

namespace ChatRoom.Core.Domain.Models;

public class Message
{
    [JsonProperty(propertyName:"id")]
    public Guid Id { get; set; }
    
    [JsonProperty(PropertyName = "dateCreated")]
    public DateTime DateCreated { get; set; }
    public string Description { get; set; }
}