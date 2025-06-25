using Google.Cloud.Storage.V1;
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

        public UploadController(GoogleCloudStorageService storageService)
        {
            _storageService = storageService;

        }
       [HttpPost]
    [Consumes("multipart/form-data")]
      
        public async Task<IActionResult> Upload([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { error = "No se subió ningún archivo." });

            using var stream = file.OpenReadStream();
            var urlPublica = await _storageService.UploadFileAsync(stream, file.FileName);

            return Ok(new { url = urlPublica });
        }


    }
}
