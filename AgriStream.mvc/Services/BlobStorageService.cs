using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using AgriStream.mvc.Models;

namespace AgriStream.mvc.Services

{
    public class BlobStorageService
    {
        private readonly BlobContainerClient _container;

        public BlobStorageService(IConfiguration config)
        {
            string connectionString = config["AzureStorage:ConnectionString"] ?? "";
            _container = new BlobContainerClient(connectionString, "drone-images");
            _container.CreateIfNotExists();
        }

        // Upload one drone image to Blob Storage
        public async Task UploadImageAsync(IFormFile file)
        {
            string blobName = Guid.NewGuid().ToString().Substring(0, 8) + "_" + Path.GetFileName(file.FileName);
            BlobClient blob = _container.GetBlobClient(blobName);

            using (Stream stream = file.OpenReadStream())
            {
                await blob.UploadAsync(stream, new BlobHttpHeaders { ContentType = file.ContentType });
            }
        }

        // Get all images (with a temporary read link so the private container still works)
        public List<DroneImageViewModel> GetImages()
        {
            var images = new List<DroneImageViewModel>();

            foreach (BlobItem item in _container.GetBlobs())
            {
                BlobClient blob = _container.GetBlobClient(item.Name);
                Uri sasUri = blob.GenerateSasUri(BlobSasPermissions.Read, DateTimeOffset.UtcNow.AddHours(1));

                var image = new DroneImageViewModel();
                image.Name = item.Name;
                image.Url = sasUri.ToString();
                image.SizeKb = Math.Round((item.Properties.ContentLength ?? 0) / 1024.0, 1);
                image.UploadedAt = item.Properties.CreatedOn;
                images.Add(image);
            }

            return images.OrderByDescending(i => i.UploadedAt).ToList();
        }
    }
}