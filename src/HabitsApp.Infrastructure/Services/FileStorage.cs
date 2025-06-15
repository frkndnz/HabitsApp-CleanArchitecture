using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HabitsApp.Application.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace HabitsApp.Infrastructure.Services;
internal class FileStorage : IFileStorage
{
    private readonly IWebHostEnvironment _webHostEnvironment;

    public FileStorage(IWebHostEnvironment webHostEnvironment)
    {
        _webHostEnvironment = webHostEnvironment;
    }

    public async Task DeleteFileAsync(string fileUrl, string folder)
    {
        if (string.IsNullOrEmpty(fileUrl))
            return;

        var fileName = Path.GetFileName(fileUrl);
        var folderPath = Path.Combine(_webHostEnvironment.WebRootPath, folder);

        var fullPath = Path.Combine(folderPath, fileName);

        if (File.Exists(fullPath))
        {
            await Task.Run(() => File.Delete(fullPath));
        }
    }

    public async Task<string> UploadFileAsync(IFormFile file, string folder, CancellationToken cancellationToken)
    {
        var uploadRootFolder=Path.Combine(_webHostEnvironment.WebRootPath?? "wwwroot",folder);

        Directory.CreateDirectory(uploadRootFolder);

        var uniqueFileName=$"{Guid.NewGuid()}_{file.FileName}";
        var filePath=Path.Combine(uploadRootFolder,uniqueFileName);

        using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream,cancellationToken);

        return Path.Combine("/", folder, uniqueFileName).Replace("\\", "/");
    }
}
