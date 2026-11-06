using Azure;
using Azure.Data.Tables;

namespace AgriStream.mvc.Models
{
    // One row in the Telemetry table
    public class TelemetryEntity : ITableEntity
    {
        public string PartitionKey { get; set; } = string.Empty; 
        public string RowKey { get; set; } = string.Empty;         
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }

        public double SoilMoisture { get; set; }
        public double Temperature { get; set; }
        public double Humidity { get; set; }
    }

    // One row in the ProcessingLogs table (written by the blob function)
    public class ProcessingLogEntity : ITableEntity
    {
        public string PartitionKey { get; set; } = string.Empty;
        public string RowKey { get; set; } = string.Empty;
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }

        public string FileName { get; set; } = string.Empty;
        public double FileSizeKb { get; set; }
        public string Status { get; set; } = string.Empty;
        public string ProcessedAt { get; set; } = string.Empty;
    }
}