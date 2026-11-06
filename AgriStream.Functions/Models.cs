namespace AgriStream.Functions
{
    // Returned by the GET function
    public class TelemetryReading
    {
        public string DeviceCode { get; set; } = "";
        public double SoilMoisture { get; set; }
        public double Temperature { get; set; }
        public double Humidity { get; set; }
        public DateTimeOffset? RecordedAt { get; set; }
    }

    // The message that goes on the queue
    public class IrrigationCommand
    {
        public string DeviceCode { get; set; } = "";
        public int Minutes { get; set; } = 10;
        public string RequestedBy { get; set; } = "";
        public string RequestedAt { get; set; } = "";
    }

    // A row written to the ProcessingLogs table
    public class ProcessingLog
    {
        public string PartitionKey { get; set; } = "DroneImage";
        public string RowKey { get; set; } = "";
        public string FileName { get; set; } = "";
        public double FileSizeKb { get; set; }
        public string Status { get; set; } = "";
        public string ProcessedAt { get; set; } = "";
    }
}