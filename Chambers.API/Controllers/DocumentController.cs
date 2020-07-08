using Chambers.API.Infrastructure.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Chambers.API.Infrastructure.Extensions;
using Microsoft.Extensions.Configuration;
using Chambers.API.Model;
using System.Collections.Generic;

namespace Chambers.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class DocumentController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IStorageService _storageService;

        public DocumentController(IConfiguration configuration,
            IStorageService storageService)
        {
            _configuration = configuration;
            _storageService = storageService;
        }

        [HttpPost]
        [Route("upload")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<ActionResult> UploadDocumentAsync(
            IFormFile file, CancellationToken cancellationToken)
        {
            if (!file.ValidateFileNameAndExtension())
                return BadRequest("Invalid file name or extension");

            long fileSizeLimit = _configuration.GetValue<long>("FileSizeLimitInBytes");
            if (!file.ValidateFileSize(fileSizeLimit))
                return BadRequest("Invalid file size");

            var fileUploadResult = await _storageService.UploadAsync(file, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { blobName = fileUploadResult.Name }, null);
        }

        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult> GetById(string blobName)
        {
            if (string.IsNullOrWhiteSpace(blobName)
                || !blobName.ToLower().EndsWith(".pdf"))
            {
                return BadRequest("Invalid file name or extension");
            }

            var stream = await _storageService.DownloadAsync(blobName);
            if (stream == null)
                return NotFound();

            string mimeType = "application/pdf";
            return new FileStreamResult(stream, mimeType)
            {
                FileDownloadName = blobName,
            };
        }

        //[HttpGet]
        //[Route("items")]
        //[ProducesResponseType(typeof(PaginatedDocuments), (int)HttpStatusCode.OK)]
        //[ProducesResponseType(typeof(IEnumerable<Document>), (int)HttpStatusCode.OK)]
        //[ProducesResponseType((int)HttpStatusCode.BadRequest)]
        //public async Task<IActionResult> ItemsAsync(
        //    [FromQuery]int pageSize = 3,
        //    [FromQuery]int pageIndex = 0)
        //{
        //    var totalItems = await _storageService.GetPagedBlobs(null, pageSize);

        //    return Ok(model);
        //}

        [HttpGet]
        [Route("items")]
        [ProducesResponseType(typeof(IEnumerable<Document>), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public async Task<IActionResult> ItemsAsync()
        {
            var blobs = await _storageService.GetBlobs();

            return Ok(blobs);
        }


        [HttpGet]
        [Route("smoketest")]
        public string Get()
        {
            return "We have take off!";
        }
    }
}
