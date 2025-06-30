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
public class BlobStorageService : IFileStorage
{
    private readonly BlobContainerClient _containerClient;

    public BlobStorageService(IConfiguration configuration)
    {
        var connectionString = configuration["AzureBlobStorage:ConnectionString"];
        var containerName = configuration["AzureBlobStorage:ContainerName"];

        _containerClient = new BlobContainerClient(connectionString, containerName);
        _containerClient.CreateIfNotExists();
    }

    public async Task DeleteFileAsync(string blobUrl, string? folder)
    {
        if (string.IsNullOrWhiteSpace(blobUrl))
            return;

        // URL'den blob adını çıkar
        string blobName = GetBlobNameFromUrl(blobUrl);

        var blobClient = _containerClient.GetBlobClient(blobName);
        Console.WriteLine($"Blob URI: {blobClient.Uri}");
        try
        {
            var response = await blobClient.DeleteIfExistsAsync();
            Console.WriteLine($"Delete success: {response.Value}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting blob: {ex.Message}");
            throw;
        }
    }

    public async Task<string> UploadFileAsync(IFormFile file, string? folder, CancellationToken cancellationToken)
    {
        var safeFileName = $"{Guid.NewGuid()}_{SanitizeFileName(file.FileName)}";
        var blobName = string.IsNullOrEmpty(folder) ? safeFileName : $"{folder}/{safeFileName}";

        var blobClient = _containerClient.GetBlobClient(blobName);

        using var stream = file.OpenReadStream();
        await blobClient.UploadAsync(stream, overwrite: true, cancellationToken);

        return blobClient.Uri.ToString();
    }
    private string SanitizeFileName(string fileName)
    {
        var invalidChars = Path.GetInvalidFileNameChars();
        return string.Concat(fileName.Where(ch => !invalidChars.Contains(ch))).Replace(" ", "_");
    }
    private static string GetBlobNameFromUrl(string blobUrl)
    {
        var uri = new Uri(blobUrl);
        var decodedPath = Uri.UnescapeDataString(uri.AbsolutePath);
        var segments = decodedPath.TrimStart('/').Split('/');
        var blobName = string.Join('/', segments.Skip(1));
        Console.WriteLine($"Extracted blob name from URL '{blobUrl}': '{blobName}'");
        return blobName;
    }
}
