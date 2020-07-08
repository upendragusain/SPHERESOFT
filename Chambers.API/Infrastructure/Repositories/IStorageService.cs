using Azure.Storage.Blobs.Models;
using Chambers.API.Model;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Chambers.API.Infrastructure.Repositories
{
    public interface IStorageService
    {
        Task<FileUploadResult> UploadAsync(IFormFile file, CancellationToken cancellationToken);

        Task<Stream> DownloadAsync(string blobFileName);

        Task<(string, List<Document>)> GetPagedBlobs(string continuationToken = null, int pageSizeHint = 3);

        Task<List<Document>> GetBlobs();
    }
}
