using Google.Cloud.Storage.V1;

namespace inventory8.Services
{
    public class GoogleCloudStorageService
    {
        private readonly StorageClient _storageClient;
        private readonly string _bucketName = "inventory8"; // Reemplaza con el tuyo

        public GoogleCloudStorageService()
        {
            _storageClient = StorageClient.Create(); // Usa la variable de entorno
        }

        public async Task<string> UploadFileAsync(Stream fileStream, string fileName)
        {
            await _storageClient.UploadObjectAsync(_bucketName, fileName, null, fileStream);
            return $"https://storage.googleapis.com/{_bucketName}/{fileName}";
        }
    }
}

