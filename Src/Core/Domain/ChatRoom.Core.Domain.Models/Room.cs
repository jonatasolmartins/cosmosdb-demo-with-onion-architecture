using ChatRoom.SharedKernel.Interfaces;
using Newtonsoft.Json;

namespace ChatRoom.Core.Domain.Models;

public class Room : IAggregateRoot
{
    [JsonProperty(propertyName:"id")]
    public Guid Id { get; set; }
    public string Name { get; set; }
    
    [JsonProperty(PropertyName = "dateCreated")]
    public string DateCreated { get; set; }
    public List<Chat> Chats { get; set; }
}