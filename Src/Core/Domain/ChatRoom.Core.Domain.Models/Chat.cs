using Newtonsoft.Json;

namespace ChatRoom.Core.Domain.Models;

public class Chat
{
    [JsonProperty(propertyName:"id")]
    public Guid Id { get; set; }
    public string Name { get; set; }
    public List<Message> Messages { get; set; }
}