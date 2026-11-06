using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Hosting;

// CHANGED: the Application Insights lines were REMOVED because the package versions clash.
// Monitoring still works in Azure through the Function App's Application Insights setting.
var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .Build();

host.Run();