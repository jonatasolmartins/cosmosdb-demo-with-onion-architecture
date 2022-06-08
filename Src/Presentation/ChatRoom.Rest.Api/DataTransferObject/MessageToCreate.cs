namespace ChatRoom.Rest.Api.DataTransferObject;

public class MessageToCreate
{
    public Guid ChatId { get; set; }
    public Guid RoomId { get; set; }
    public string Content { get; set; }
}