using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Chambers.API.Infrastructure.Repositories;
using Chambers.API.Model;
using Microsoft.AspNetCore.Http;
using Serilog;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Chambers.API.Infrastructure
{
    public class AzureBlobStorageService : IStorageService
    {
        private readonly string _connectionString;
        private readonly string _containerName;

        private const string FILE_EXTENSION_PDF = ".pdf";

        public AzureBlobStorageService(string connectionString, string containerName)
        {
            _connectionString = connectionString;
            _containerName = containerName;
        }

        public async Task<FileUploadResult> UploadAsync(IFormFile file, CancellationToken cancellationToken)
        {
            var blobContainerClient = await GetBlobContainerClient();
            string uniqueFileName = Guid.NewGuid().ToString() + FILE_EXTENSION_PDF;
            BlobClient blobClient = blobContainerClient.GetBlobClient(uniqueFileName);
            await blobClient.UploadAsync(file.OpenReadStream());
            return new FileUploadResult(name: uniqueFileName, url: blobClient.Uri.ToString());
        }

        public async Task<Stream> DownloadAsync(string blobFileName)
        {
            var blobContainerClient = await GetBlobContainerClient();
            BlobClient blobClient = blobContainerClient.GetBlobClient(blobFileName);
            BlobDownloadInfo blob = await blobClient.DownloadAsync();
            return blob.Content;
        }

        private async Task<BlobContainerClient> GetBlobContainerClient()
        {
            // Create a BlobServiceClient object which will be used to create a container client
            BlobServiceClient blobServiceClient = new BlobServiceClient(_connectionString);

            // Create the container and return a container client object
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(_containerName);

            await containerClient.CreateIfNotExistsAsync();

            return containerClient;
        }
    }
}
