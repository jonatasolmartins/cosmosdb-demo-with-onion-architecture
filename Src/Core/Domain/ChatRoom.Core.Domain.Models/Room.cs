using Newtonsoft.Json;

namespace ChatRoom.Core.Domain.Models;

public class Room
{
    [JsonProperty(propertyName:"id")]
    public Guid Id { get; set; }
    public string Name { get; set; }
    public List<Chat> Chats { get; set; }
}