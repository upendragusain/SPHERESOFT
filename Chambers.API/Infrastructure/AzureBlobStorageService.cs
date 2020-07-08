using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Chambers.API.Infrastructure.Repositories;
using Chambers.API.Model;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
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

        //todo: implement this properly
        public async Task<(string,List<Document>)> GetPagedBlobs(string continuationToken = null, int pageSizeHint = 3)
        {
            var blobContainerClient = await GetBlobContainerClient();
            var resultSegment = blobContainerClient.GetBlobsAsync(BlobTraits.All, BlobStates.All)
                .AsPages(continuationToken, pageSizeHint: pageSizeHint);

            var documents = new List<Document>();
            await foreach (Azure.Page<BlobItem> blobPage in resultSegment)
            {
                continuationToken = blobPage.ContinuationToken;
                foreach (BlobItem blobItem in blobPage.Values)
                {
                    BlobClient blobClient = blobContainerClient.GetBlobClient(blobItem.Name);
                    var properties = blobClient.GetProperties().Value;
                    documents.Add(new Document(blobItem.Name, 
                        blobClient.Uri.ToString(),
                        properties.ContentLength));
                }
            }
            return (continuationToken, documents);
        }

        public async Task<List<Document>> GetBlobs()
        {
            var blobContainerClient = await GetBlobContainerClient();
            var documents = new List<Document>();
            foreach (BlobItem blobItem in blobContainerClient.GetBlobs())
            {
                BlobClient blobClient = blobContainerClient.GetBlobClient(blobItem.Name);
                var properties = blobClient.GetProperties().Value;
                documents.Add(new Document(blobItem.Name,
                    blobClient.Uri.ToString(),
                    properties.ContentLength));
            }
            
            return documents;
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
