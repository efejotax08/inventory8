using Google.Cloud.Storage.V1;
using inventory8.Entities;
using inventory8.Services;
using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace inventory8.Controllers
{


    [ApiController]
    [Route("api/[controller]")]
    public class UploadController : Controller
    {
        private readonly GoogleCloudStorageService _storageService;

        public UploadController()
        {
            _storageService = new GoogleCloudStorageService();

        }
     [HttpPost]
    [Route("image")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadImage([FromForm] UploadImageRequest request)
    {
            if (request.File == null || request.File.Length == 0)
                return BadRequest("No se subió archivo.");

            var imageUrl = await _storageService.UploadImageAsync(request.File);
            return Ok(new { url = imageUrl });
        }
     
    }
    public class UploadImageRequest
    {
        public IFormFile File { get; set; }
    }
}
