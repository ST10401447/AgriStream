using AgriStream.mvc.Models;
using AgriStream.mvc.Models;
using AgriStream.mvc.Models;
using System.ComponentModel.DataAnnotations;

namespace AgriStream.mvc.Models
{
    public class RegisterViewModel
    {
        [Required, StringLength(50, MinimumLength = 3)]
        public string Username { get; set; } = string.Empty;

        [Required, StringLength(100, MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        [Display(Name = "Confirm password")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required]
        public string Role { get; set; } = "Farmer";
    }

    public class LoginViewModel
    {
        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }

    // Data that comes back from the GET function
    public class TelemetryReading
    {
        public string DeviceCode { get; set; } = string.Empty;
        public double SoilMoisture { get; set; }
        public double Temperature { get; set; }
        public double Humidity { get; set; }
        public DateTimeOffset? RecordedAt { get; set; }
    }

    public class DashboardViewModel
    {
        public List<Device> Devices { get; set; } = new List<Device>();
        public string SelectedDeviceCode { get; set; } = string.Empty;
        public List<TelemetryReading> Readings { get; set; } = new List<TelemetryReading>();
        public string ErrorMessage { get; set; } = string.Empty;
    }

    public class DroneImageViewModel
    {
        public string Name { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public double SizeKb { get; set; }
        public DateTimeOffset? UploadedAt { get; set; }
    }

    public class ImagesViewModel
    {
        public List<DroneImageViewModel> Images { get; set; } = new List<DroneImageViewModel>();
        public List<ProcessingLogEntity> Logs { get; set; } = new List<ProcessingLogEntity>();
    }
}