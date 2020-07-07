using Chambers.API.Infrastructure.Repositories;

namespace Chambers.API.Infrastructure
{
    public class FileUploadRepository : IFileUploadRepository
    {
        private readonly IStorageService _storageService;

        public FileUploadRepository(IStorageService storageService)
        {
            this._storageService = storageService;
        }
    }
}
