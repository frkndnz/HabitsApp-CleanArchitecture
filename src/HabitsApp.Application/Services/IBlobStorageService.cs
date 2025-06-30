using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace HabitsApp.Application.Services;
public interface IBlobStorageService
{
    Task<string> UploadFileAsync(IFormFile file,CancellationToken cancellationToken);
    Task DeleteFileAsync(string blobUrl, CancellationToken cancellationToken);
}
