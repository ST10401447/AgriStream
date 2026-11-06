using AgriStream.mvc.Services;
using AgriStream.mvc.Models;
using AgriStream.mvc.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgriStream.mvc.Controllers
{
    [Authorize]
    public class ImagesController : Controller
    {
        private readonly BlobStorageService _blobService;
        private readonly TableStorageService _tableService;

        public ImagesController(BlobStorageService blobService, TableStorageService tableService)
        {
            _blobService = blobService;
            _tableService = tableService;
        }

        public async Task<IActionResult> Index()
        {
            var model = new ImagesViewModel();
            model.Images = _blobService.GetImages();
            model.Logs = await _tableService.GetLatestLogsAsync();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile? imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
            {
                TempData["Error"] = "Please choose an image first.";
                return RedirectToAction("Index");
            }

            if (!imageFile.ContentType.StartsWith("image/"))
            {
                TempData["Error"] = "Only image files are allowed.";
                return RedirectToAction("Index");
            }

            if (imageFile.Length > 10 * 1024 * 1024)
            {
                TempData["Error"] = "The image must be smaller than 10 MB.";
                return RedirectToAction("Index");
            }

            await _blobService.UploadImageAsync(imageFile);

            TempData["Message"] = "Image uploaded. The blob function will log it shortly.";
            return RedirectToAction("Index");
        }
    }
}