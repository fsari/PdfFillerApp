using iText.Forms;
using iText.Kernel.Pdf;
using Microsoft.AspNetCore.Mvc;

namespace PdfFillerApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        IWebHostEnvironment _env;

        public HomeController(ILogger<HomeController> logger, IWebHostEnvironment env)
        {
            _logger = logger;
            _env = env;
        }

        public IActionResult Index()
        {
            return View();
        }
        public void FillPdf()
        {
            string inputPdfPath = @"wwwroot\temp\orginalform.pdf";
            string outputPdfPath = @"wwwroot\temp\" + Guid.NewGuid().ToString() + ".pdf";

            using PdfReader pdfReader = new(inputPdfPath);
            using PdfWriter pdfWriter = new(outputPdfPath);
            using PdfDocument pdfDocument = new(pdfReader, pdfWriter);
            PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDocument, true);

            // Replace "fieldName" with the actual field name in your PDF
            form.GetField("name").SetValue("Faruk");
            form.GetField("antragsteller").SetValue("Faruk SARI");
            form.GetField("akademischer_grad").SetValue("Faruk SARI PHD");

            // Add more lines like the one above for each field you want to fill

            pdfDocument.Close();

            Console.WriteLine("PDF filled successfully.");
            // return outputPdfPath;


        }


    }
}
