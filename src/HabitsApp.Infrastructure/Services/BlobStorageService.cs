using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using HabitsApp.Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace HabitsApp.Infrastructure.Services;
public class BlobStorageService : IBlobStorageService
{
    private readonly BlobContainerClient _containerClient;

    public BlobStorageService(IConfiguration configuration)
    {
        var connectionString = configuration["AzureBlobStorage:ConnectionString"];
        var containerName = configuration["AzureBlobStorage:ContainerName"];

        _containerClient = new BlobContainerClient(connectionString, containerName);
        _containerClient.CreateIfNotExists();
    }

    public async Task DeleteFileAsync(string blobUrl, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(blobUrl))
            return;

        // Blob URL'sinden blob adı/path çıkarılıyor
        var uri = new Uri(blobUrl);
        var blobName = uri.Segments.Skip(2).Aggregate((a, b) => a + b);
        // Segments[0] = "/", [1] = "container/", sonrası blob path

        var blobClient = _containerClient.GetBlobClient(blobName);

        await blobClient.DeleteIfExistsAsync();
    }

    public async Task<string> UploadFileAsync(IFormFile file, CancellationToken cancellationToken)
    {
        var safeFileName = $"{Guid.NewGuid()}_{SanitizeFileName(file.FileName)}";
        var blobClient = _containerClient.GetBlobClient(safeFileName);

        using var stream = file.OpenReadStream();
        await blobClient.UploadAsync(stream, overwrite: true, cancellationToken);

        return blobClient.Uri.ToString(); // Bu URL frontend tarafında kullanılabilir
    }
    private string SanitizeFileName(string fileName)
    {
        var invalidChars = Path.GetInvalidFileNameChars();
        return string.Concat(fileName.Where(ch => !invalidChars.Contains(ch))).Replace(" ", "_");
    }
}
