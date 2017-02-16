using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace First_App.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }

        public IActionResult Uploads()
        {
            return View();
        }

        public IActionResult TagHelpers()
        {
            return View();
        }

        public IActionResult ViewComponents()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UploadSmallSingleFile(IFormFile file)
        {
            var size = file.Length;

            // full path to file in temp location
            var filePath = Path.GetTempFileName();

            if (size <= 0) return Ok(new {size, filePath});

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // process uploaded files
            // Don't rely on or trust the FileName property without validation.

            return Ok(new { size, filePath });
        }

        [HttpPost]
        public async Task<IActionResult> UploadSmallMultipleFiles(List<IFormFile> files)
        {
            var size = files.Sum(f => f.Length);

            // full path to file in temp location
            var filePath = Path.GetTempFileName();

            foreach (var formFile in files)
            {
                if (formFile.Length <= 0) continue;

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await formFile.CopyToAsync(stream);
                }
            }

            // process uploaded files
            // Don't rely on or trust the FileName property without validation.

            return Ok(new { count = files.Count, size, filePath });
        }
    }
}
