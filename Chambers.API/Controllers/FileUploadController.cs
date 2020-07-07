using Chambers.API.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Chambers.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class FileUploadController : ControllerBase
    {
        private readonly ILogger<FileUploadController> _logger;
        private readonly IFileUploadRepository _fileUploadRepository;

        public FileUploadController(ILogger<FileUploadController> logger,
            IFileUploadRepository fileUploadRepository)
        {
            _logger = logger;
            _fileUploadRepository = fileUploadRepository;
        }

        [HttpGet]
        public string Get()
        {
            return "We have take off!";
        }
    }
}
