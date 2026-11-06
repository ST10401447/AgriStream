
using Azure.Data.Tables;
using AgriStream.mvc.Models;

namespace AgriStream.mvc.Services

{
    public class TableStorageService
    {
        private readonly TableClient _telemetryTable;
        private readonly TableClient _logsTable;

        public TableStorageService(IConfiguration config)
        {
            string connectionString = config["AzureStorage:ConnectionString"] ?? "";
            var serviceClient = new TableServiceClient(connectionString);

            _telemetryTable = serviceClient.GetTableClient("Telemetry");
            _telemetryTable.CreateIfNotExists();

            _logsTable = serviceClient.GetTableClient("ProcessingLogs");
            _logsTable.CreateIfNotExists();
        }

        // Saves one sensor reading (this simulates a sensor sending data)
        public async Task AddReadingAsync(string deviceCode, double soilMoisture, double temperature, double humidity)
        {
            var entity = new TelemetryEntity();
            entity.PartitionKey = deviceCode;
            // Reverse ticks make the newest reading come first when we query
            entity.RowKey = (DateTime.MaxValue.Ticks - DateTime.UtcNow.Ticks).ToString("d19");
            entity.SoilMoisture = soilMoisture;
            entity.Temperature = temperature;
            entity.Humidity = humidity;

            await _telemetryTable.AddEntityAsync(entity);
        }

        // Gets the latest 10 processing log entries written by the blob function
        public async Task<List<ProcessingLogEntity>> GetLatestLogsAsync()
        {
            var logs = new List<ProcessingLogEntity>();
            var rows = _logsTable.QueryAsync<ProcessingLogEntity>(filter: "PartitionKey eq 'DroneImage'", maxPerPage: 10);

            await foreach (ProcessingLogEntity row in rows)
            {
                logs.Add(row);
                if (logs.Count >= 10)
                {
                    break;
                }
            }

            return logs;
        }
    }
}