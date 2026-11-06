using System.Net.Http.Json;
using System.Text; // ADDED: needed for Encoding.UTF8
using System.Text.Json; // ADDED: needed for JsonSerializer
using AgriStream.mvc.Models;

namespace AgriStream.mvc.Services
{
    // Calls our Azure Functions over HTTP
    public class FunctionApiService
    {
        private readonly HttpClient _http;

        public FunctionApiService(HttpClient http, IConfiguration config)
        {
            _http = http;
            _http.BaseAddress = new Uri(config["FunctionApi:BaseUrl"] ?? "http://localhost:7234/");

            string key = config["FunctionApi:Key"] ?? "";
            if (key != "")
            {
                _http.DefaultRequestHeaders.Add("x-functions-key", key);
            }
        }

        // HTTP GET function: latest telemetry
        public async Task<List<TelemetryReading>> GetTelemetryAsync(string deviceCode)
        {
            var result = await _http.GetFromJsonAsync<List<TelemetryReading>>("api/telemetry/" + deviceCode);
            if (result == null)
            {
                return new List<TelemetryReading>();
            }
            return result;
        }

        // HTTP POST function: puts an irrigation command on the queue
        public async Task<bool> TriggerIrrigationAsync(string deviceCode, int minutes, string requestedBy)
        {
            var command = new { DeviceCode = deviceCode, Minutes = minutes, RequestedBy = requestedBy };

            // CHANGED: send plain JSON text so the body has a fixed length (PostAsJsonAsync was removed)
            string json = JsonSerializer.Serialize(command);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _http.PostAsync("api/irrigation", content);
            return response.IsSuccessStatusCode;
        }
    }
}