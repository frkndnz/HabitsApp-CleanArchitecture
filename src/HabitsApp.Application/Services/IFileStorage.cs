using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace HabitsApp.Application.Services;
public interface IFileStorage
{
    Task<string> UploadFileAsync(IFormFile file,string? folder,CancellationToken cancellationToken);
    Task DeleteFileAsync(string fileUrl, string? folder);
}
