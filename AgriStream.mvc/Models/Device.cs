namespace AgriStream.mvc.Models
{
    // Master registry of devices (the queue function updates Status)
    public class Device
    {
        public int DeviceId { get; set; }
        public string DeviceCode { get; set; } = string.Empty;
        public string FieldName { get; set; } = string.Empty;
        public string Status { get; set; } = "Idle";
        public DateTime LastUpdated { get; set; }
    }
}