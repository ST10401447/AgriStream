using System.Diagnostics;

Console.WriteLine("=== AgriStream Load Test ===");
Console.Write("Site URL (example https://st10401447.azurewebsites.net): ");
string baseUrl = (Console.ReadLine() ?? "").TrimEnd('/');

Console.Write("Number of concurrent users (example 100): ");
int users = int.Parse(Console.ReadLine() ?? "100");

Console.Write("How many minutes to run (example 10): ");
int minutes = int.Parse(Console.ReadLine() ?? "10");

var client = new HttpClient();
client.Timeout = TimeSpan.FromSeconds(60);

DateTime endTime = DateTime.Now.AddMinutes(minutes);
var timer = Stopwatch.StartNew();

Console.WriteLine("Starting " + users + " users for " + minutes + " minutes...");

// Start all the fake users at the same time
var tasks = new List<Task>();
for (int i = 0; i < users; i++)
{
    tasks.Add(SimulateUser());
}
await Task.WhenAll(tasks);
timer.Stop();

// Print the results (screenshot this for your document)
Console.WriteLine();
Console.WriteLine("=== RESULTS ===");
Console.WriteLine("Total requests : " + (Counters.Success + Counters.Failed));
Console.WriteLine("Successful     : " + Counters.Success);
Console.WriteLine("Failed         : " + Counters.Failed);
if (Counters.Success > 0)
{
    Console.WriteLine("Average time   : " + (Counters.TotalMs / Counters.Success) + " ms");
}
Console.WriteLine("Requests/second: " + Math.Round((Counters.Success + Counters.Failed) / timer.Elapsed.TotalSeconds, 1));
Console.WriteLine("Press Enter to close.");
Console.ReadLine();

// One fake user keeps calling the site until the time is over
async Task SimulateUser()
{
    while (DateTime.Now < endTime)
    {
        var watch = Stopwatch.StartNew();
        try
        {
            HttpResponseMessage response = await client.GetAsync(baseUrl + "/Home/Compute");
            watch.Stop();

            if (response.IsSuccessStatusCode)
            {
                Interlocked.Increment(ref Counters.Success);
                Interlocked.Add(ref Counters.TotalMs, watch.ElapsedMilliseconds);
            }
            else
            {
                Interlocked.Increment(ref Counters.Failed);
            }
        }
        catch (Exception)
        {
            Interlocked.Increment(ref Counters.Failed);
        }
    }
}

// Shared counters
static class Counters
{
    public static int Success = 0;
    public static int Failed = 0;
    public static long TotalMs = 0;
}