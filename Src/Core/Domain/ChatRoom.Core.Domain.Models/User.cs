using Newtonsoft.Json;

namespace ChatRoom.Core.Domain.Models;

public class User
{
    [JsonProperty(propertyName:"id")]
    public Guid Id { get; set; }
    public string Name { get; set; }
    [JsonProperty(propertyName:"email")]
    public string Email { get; set; }
    public string Avatar { get; set; }
}