using System.Net;
using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace AgriStream.Functions
{
    // Two outputs: a message for the queue and the HTTP answer
    public class TriggerIrrigationOutput
    {
        [QueueOutput("irrigation-commands", Connection = "AzureWebJobsStorage")]
        public string? QueueMessage { get; set; }

        public HttpResponseData? HttpResponse { get; set; }
    }

    public class TriggerIrrigation
    {
        // TRIGGER: HTTP POST  /api/irrigation
        // OUTPUT BINDING: puts the irrigation command on the Storage Queue
        [Function("TriggerIrrigation")]
        public async Task<TriggerIrrigationOutput> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "irrigation")] HttpRequestData req)
        {
            var output = new TriggerIrrigationOutput();

            // CHANGED: read the body as text first (ReadFromJsonAsync crashed on an empty body)
            string body = await req.ReadAsStringAsync() ?? "";

            // ADDED: stop early if the body is empty
            if (body == "")
            {
                var emptyResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                await emptyResponse.WriteStringAsync("The request body was empty.");
                output.HttpResponse = emptyResponse;
                return output;
            }

            // ADDED: turn the text into an IrrigationCommand (ignore upper/lower case in names)
            var options = new JsonSerializerOptions();
            options.PropertyNameCaseInsensitive = true;
            IrrigationCommand? command = JsonSerializer.Deserialize<IrrigationCommand>(body, options);

            if (command == null || string.IsNullOrEmpty(command.DeviceCode))
            {
                var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                await badResponse.WriteStringAsync("Please send a DeviceCode.");
                output.HttpResponse = badResponse;
                return output;
            }

            command.RequestedAt = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
            output.QueueMessage = JsonSerializer.Serialize(command);

            var response = req.CreateResponse(HttpStatusCode.Accepted);
            await response.WriteStringAsync("Irrigation command queued for " + command.DeviceCode);
            output.HttpResponse = response;
            return output;
        }
    }
}