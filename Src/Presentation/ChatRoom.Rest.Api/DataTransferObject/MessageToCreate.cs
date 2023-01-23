namespace ChatRoom.Rest.Api.DataTransferObject;

public class MessageToCreate
{
    public Guid Id { get; set; }
    public Guid ChatId { get; set; }
    public Guid RoomId { get; set; }
    public string Content { get; set; }
    public UserMessage User { get; set; }
}