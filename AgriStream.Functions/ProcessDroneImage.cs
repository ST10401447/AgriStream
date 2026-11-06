using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace AgriStream.Functions
{
    public class ProcessDroneImage
    {
        private readonly ILogger<ProcessDroneImage> _logger;

        public ProcessDroneImage(ILogger<ProcessDroneImage> logger)
        {
            _logger = logger;
        }

        // TRIGGER: runs when a new blob lands in the drone-images container
        // OUTPUT BINDING: the returned object is saved into the ProcessingLogs table
        [Function("ProcessDroneImage")]
        [TableOutput("ProcessingLogs", Connection = "AzureWebJobsStorage")]
        public ProcessingLog Run(
            [BlobTrigger("drone-images/{name}", Connection = "AzureWebJobsStorage")] byte[] imageBytes,
            string name)
        {
            _logger.LogInformation("New drone image received: {name} ({size} bytes)", name, imageBytes.Length);

            var log = new ProcessingLog();
            log.PartitionKey = "DroneImage";
            // Reverse ticks so the newest log is first in the table
            log.RowKey = (DateTime.MaxValue.Ticks - DateTime.UtcNow.Ticks).ToString("d19");
            log.FileName = name;
            log.FileSizeKb = Math.Round(imageBytes.Length / 1024.0, 1);
            log.Status = "Processed";
            log.ProcessedAt = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");

            return log;
        }
    }
}