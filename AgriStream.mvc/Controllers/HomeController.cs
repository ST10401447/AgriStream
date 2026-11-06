using System.Diagnostics;
using System.Security.Cryptography; 
using AgriStream.mvc.Models;
using Microsoft.AspNetCore.Mvc;

namespace AgriStream.mvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        // ADDED: used by the load test. It does some CPU work so we can see the app scale out (Task e and g)
        public IActionResult Compute()
        {
            byte[] data = new byte[1024];
            for (int i = 0; i < 50000; i++)
            {
                data = SHA256.HashData(data);
            }
            return Content("Done");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}