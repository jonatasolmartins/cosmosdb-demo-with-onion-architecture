using System.Text.RegularExpressions;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using ChatRoom.Core.Domain.Abstractions.Repositories;

namespace ChatRoom.Infrastructure.Database.Implementations;

public class MultimediaRepository : IMultimediaRepository
{
    private readonly BlobServiceClient _blobServiceClient;

    public MultimediaRepository(BlobServiceClient blobServiceClient)
    {
        _blobServiceClient = blobServiceClient;
    }

    public async Task<string> UploadImage(string imageBase64Format, string container, string fileName)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(container);
        var blobClient = containerClient.GetBlobClient(fileName);

        var data = new Regex(@"^data:image\/[a-z]+;base64,").Replace(imageBase64Format, "");

        var imageBytes = Convert.FromBase64String(data);

        await using var stream = new MemoryStream(imageBytes);

        await blobClient.UploadAsync(stream, new BlobHttpHeaders {ContentType = "jpg"});

        return blobClient.Uri.AbsoluteUri;
    }
}