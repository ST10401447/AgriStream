using AgriStream.mvc.Data;
using AgriStream.mvc.Models;
using AgriStream.mvc.Services;
using AgriStream.mvc.Data;
using AgriStream.mvc.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AgriStream.mvc.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly AppDbContext _context;
        private readonly FunctionApiService _functionApi;
        private readonly TableStorageService _tableService;

        public DashboardController(AppDbContext context, FunctionApiService functionApi, TableStorageService tableService)
        {
            _context = context;
            _functionApi = functionApi;
            _tableService = tableService;
        }

        public async Task<IActionResult> Index(string? deviceCode)
        {
            var model = new DashboardViewModel();
            model.Devices = await _context.Devices.OrderBy(d => d.DeviceCode).ToListAsync();

            if (string.IsNullOrEmpty(deviceCode) && model.Devices.Count > 0)
            {
                deviceCode = model.Devices[0].DeviceCode;
            }
            model.SelectedDeviceCode = deviceCode ?? "";

            // Ask the HTTP GET function for the latest telemetry
            try
            {
                model.Readings = await _functionApi.GetTelemetryAsync(model.SelectedDeviceCode);
            }
            catch (Exception ex)
            {
                model.ErrorMessage = "Could not load telemetry: " + ex.Message;
            }

            return View(model);
        }

        // Simulates a sensor sending a new reading into Table Storage
        [HttpPost]
        public async Task<IActionResult> AddReading(string deviceCode)
        {
            var random = new Random();
            double soil = Math.Round(random.NextDouble() * 60 + 10, 1);
            double temp = Math.Round(random.NextDouble() * 20 + 15, 1);
            double humidity = Math.Round(random.NextDouble() * 50 + 30, 1);

            await _tableService.AddReadingAsync(deviceCode, soil, temp, humidity);

            TempData["Message"] = "New sensor reading saved for " + deviceCode + ".";
            return RedirectToAction("Index", new { deviceCode = deviceCode });
        }

        // Only Farmers and Admins may start irrigation
        [HttpPost]
        [Authorize(Roles = "Farmer,Admin")]
        public async Task<IActionResult> TriggerIrrigation(string deviceCode, int minutes)
        {
            string userName = User.Identity?.Name ?? "unknown";
            bool ok = false;

            try
            {
                ok = await _functionApi.TriggerIrrigationAsync(deviceCode, minutes, userName);
            }
            catch (Exception)
            {
                ok = false;
            }

            if (ok)
            {
                TempData["Message"] = "Irrigation command sent to the queue for " + deviceCode + ". Refresh in a few seconds to see the new status.";
            }
            else
            {
                TempData["Error"] = "The irrigation command could not be sent.";
            }

            return RedirectToAction("Index", new { deviceCode = deviceCode });
        }
    }
}