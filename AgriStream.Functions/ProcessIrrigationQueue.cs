using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace AgriStream.Functions
{
    public class ProcessIrrigationQueue
    {
        private readonly ILogger<ProcessIrrigationQueue> _logger;

        public ProcessIrrigationQueue(ILogger<ProcessIrrigationQueue> logger)
        {
            _logger = logger;
        }

        // TRIGGER: runs when a message appears in the irrigation-commands queue
        [Function("ProcessIrrigationQueue")]
        public async Task Run(
            [QueueTrigger("irrigation-commands", Connection = "AzureWebJobsStorage")] string message)
        {
            IrrigationCommand? command = JsonSerializer.Deserialize<IrrigationCommand>(message);
            if (command == null)
            {
                _logger.LogWarning("Could not read the queue message.");
                return;
            }

            string connectionString = Environment.GetEnvironmentVariable("SqlConnectionString") ?? "";

            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                string sql = "UPDATE Devices SET Status = @status, LastUpdated = @now WHERE DeviceCode = @code";
                using (var cmd = new SqlCommand(sql, connection))
                {
                    cmd.Parameters.AddWithValue("@status", "Irrigating for " + command.Minutes + " min (by " + command.RequestedBy + ")");
                    cmd.Parameters.AddWithValue("@now", DateTime.UtcNow);
                    cmd.Parameters.AddWithValue("@code", command.DeviceCode);

                    int rows = await cmd.ExecuteNonQueryAsync();
                    _logger.LogInformation("Irrigation started for {code}. Rows updated: {rows}", command.DeviceCode, rows);
                }
            }
        }
    }
}