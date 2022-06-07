namespace ChatRoom.Rest.Api.DataTransferObject;

public class MessageToCreate
{
    public string Description { get; set; }
    public string ChatId { get; set; }
    public string RoomID { get; set; }
}