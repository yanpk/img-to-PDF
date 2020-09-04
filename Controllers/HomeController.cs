using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using TesteIFrame.Models;

namespace TesteIFrame.Controllers
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
            try
            {
                var directory = Directory.GetCurrentDirectory();
                var inicializeDocFolder = Directory.Exists($@"{Directory.GetCurrentDirectory()}\wwwroot\doc");
                if (!inicializeDocFolder)
                    Directory.CreateDirectory($@"{Directory.GetCurrentDirectory()}\wwwroot\doc");

                var imagem = Image.FromFile($@"{Directory.GetCurrentDirectory()}\wwwroot\img\ddt s31.png");
                var bytes = ImageToByteArray(imagem);
                var pdf = ImageConverter(bytes);
                ViewBag.Url = pdf;
            }
            catch (Exception ex)
            {
                
            }
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public byte[] ImageToByteArray(System.Drawing.Image imageIn)
        {
            using (var ms = new MemoryStream())
            {
                imageIn.Save(ms, imageIn.RawFormat);
                return ms.ToArray();
            }
        }

        public string ImageConverter(byte[] img)
        {
            var file = File(img, "image/jpg");
            Stream stream = new MemoryStream(img);

            Image image = null;
            using (var ms = new MemoryStream(img))
            {
                ms.Position = 0;
                image = Bitmap.FromStream(stream);
            }

            var guid = Guid.NewGuid().ToString();
            var pathImg = $@"{Directory.GetCurrentDirectory()}\wwwroot\img\{guid}.jpg";
            image.Save(pathImg);

            var pathPdf = $@"{Directory.GetCurrentDirectory()}\wwwroot\doc\{guid}.pdf";
            var arquivo = $@"{guid}.pdf";

            GeneratePDF(pathPdf, $@"{Directory.GetCurrentDirectory()}\wwwroot\img\{guid}.jpg");

            image.Dispose();

            if (System.IO.File.Exists(pathImg))
            {
                System.IO.File.Delete(pathImg);
            }

            return arquivo;
        }

        private void GeneratePDF(string filename, string imageLoc)
        {
            PdfDocument document = new PdfDocument();

            var img = Image.FromFile(imageLoc);

            PdfPage page = document.AddPage();
            page.Width = img.Width;
            page.Height = img.Height;
            XGraphics gfx = XGraphics.FromPdfPage(page);
            DrawImage(gfx, imageLoc, 0, 0, img.Width, img.Height);

            document.Save(filename);
            document.Close();
            img.Dispose();
            document.Dispose();
        }

        void DrawImage(XGraphics gfx, string jpegSamplePath, int x, int y, int width, int height)
        {
            XImage image = XImage.FromFile(jpegSamplePath);
            gfx.DrawImage(image, x, y, width, height);
            image.Dispose();
        }
    }
}
