using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using mvc_controls.Models;
using System.IO;

namespace mvc_controls.Controllers;

public class HomeController : Controller
{
    private IWebHostEnvironment _hostEnvironment;
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger, IWebHostEnvironment hostEnvironment)
    {
        _logger = logger;
        _hostEnvironment = hostEnvironment;
    }

    public IActionResult Index()
    {

        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }



    [HttpGet]
    public List<ImageModel> GetUrlImage()
    {
        // fetch all files in the folder
        string[] filePaths = Directory.GetFiles(Path.Combine(_hostEnvironment.WebRootPath, "upload/dropzone"));
        List<ImageModel> images = new List<ImageModel>();
        foreach (string filePath in filePaths)
        {
            images.Add(new ImageModel
            {
                Name = Path.GetFileName(filePath),
                Url = Request.Scheme + "://" + Request.Host + "/upload/dropzone/" + Path.GetFileName(filePath),
                Size = new FileInfo(filePath).Length
            });
        }
        return images;
    }

    public async Task<IActionResult> UploadFiles(List<IFormFile> files, string test)
    {
        bool isSuccess = true;
        string _ = $@"\upload\dropzone";
        if (files.Count() > 0)
        {
            if (!Directory.Exists(_hostEnvironment.WebRootPath + _))
            {
                Directory.CreateDirectory(_hostEnvironment.WebRootPath + _);
            }
            try
            {
                var md5 = System.Security.Cryptography.MD5.Create();
                foreach (IFormFile file in files)
                {


                    using var stream = new FileStream(
                        _hostEnvironment.WebRootPath
                        + _
                        + "\\"
                        + file.FileName.Trim(),
                        FileMode.Create);
                    await file.CopyToAsync(stream);
                    var result = BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", "");
                }
            }
            catch (System.Exception ex)
            {
                isSuccess = false;
            }
        }
        if (isSuccess)
        {
            return Json("");
        }
        else
        {
            return RedirectToAction("Index", "No files were uploaded!");
        }
    }

    public IActionResult RemoveFile(string fileName)
    {
        string path = Path.Combine(_hostEnvironment.WebRootPath, "upload/dropzone");
        if (!string.IsNullOrEmpty(fileName))
            while (System.IO.File.Exists(Path.Combine(path, fileName)))
            {
                System.IO.File.Delete(Path.Combine(path, fileName));
            }
        return Ok("");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

public class ImageModel
{
    public string Name { get; set; }
    public string? Url { get; set; }

    public long Size { get; set; }
}
