namespace ChatRoom.Rest.Api.DataTransferObject;

public class UserMessage
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Avatar { get; set; }
}