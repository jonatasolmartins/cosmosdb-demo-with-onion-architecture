namespace ChatRoom.Core.Domain.Abstractions.Repositories;

public interface IMultimediaRepository
{
    Task<string> UploadImage(string imageBase64Format, string container, string fileName);
}