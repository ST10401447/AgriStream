using System.Net;
using Azure.Data.Tables;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace AgriStream.Functions
{
    public class GetTelemetry
    {
        // TRIGGER: HTTP GET  /api/telemetry/{deviceCode}
        // INPUT BINDING: the TableClient for the Telemetry table is given to us
        [Function("GetTelemetry")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "telemetry/{deviceCode}")] HttpRequestData req,
            string deviceCode,
            [TableInput("Telemetry", Connection = "AzureWebJobsStorage")] TableClient telemetryTable)
        {
            string safeCode = deviceCode.Replace("'", "''");
            var readings = new List<TelemetryReading>();

            var rows = telemetryTable.QueryAsync<TableEntity>(filter: "PartitionKey eq '" + safeCode + "'", maxPerPage: 10);

            await foreach (TableEntity row in rows)
            {
                var reading = new TelemetryReading();
                reading.DeviceCode = row.PartitionKey;
                reading.SoilMoisture = row.GetDouble("SoilMoisture") ?? 0;
                reading.Temperature = row.GetDouble("Temperature") ?? 0;
                reading.Humidity = row.GetDouble("Humidity") ?? 0;
                reading.RecordedAt = row.Timestamp;
                readings.Add(reading);

                if (readings.Count >= 10)
                {
                    break;
                }
            }

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(readings);
            return response;
        }
    }
}