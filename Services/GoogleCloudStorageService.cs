using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;

namespace inventory8.Services
{
    public class GoogleCloudStorageService
    {
        private readonly StorageClient _storageClient;
        private readonly string _bucketName = "inventory8bucket";

        public GoogleCloudStorageService()
        {
            _storageClient = StorageClient.Create();
        }

        public async Task<string> UploadFileAsync(Stream fileStream, string fileName)
        {
            await _storageClient.UploadObjectAsync(_bucketName, fileName, null, fileStream);

            // IMPORTANTE: El bucket debe ser público o el objeto debe hacerse público
            return $"https://storage.googleapis.com/{_bucketName}/{fileName}";
        }

        public async Task<string> UploadImageAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("Archivo no válido");

            var objectName = Guid.NewGuid() + Path.GetExtension(file.FileName); // nombre único
            using var stream = file.OpenReadStream();

            var obj = await _storageClient.UploadObjectAsync(_bucketName, objectName, file.ContentType, stream);

            return $"https://storage.googleapis.com/{_bucketName}/{objectName}";
        }
    }
}

