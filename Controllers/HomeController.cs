using iText.Forms;
using iText.Kernel.Pdf;
using Microsoft.AspNetCore.Mvc;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using PdfFillerApp.Models;
using System.Data;

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



        public async Task<PdfAcroForm> FillForm(PdfAcroForm form, PdfVM data)
        {

            // Replace "fieldName" with the actual field name in your PDF
            form.GetField("name").SetValue(data.name);
            form.GetField("vorname").SetValue(data.vorname);
            form.GetField("antragsteller").SetValue(data.antragsteller);
            form.GetField("akademischer_grad").SetValue(data.akademischer_grad);

            return form;
        }



        public async Task FillPdf()
        {

            string inputPdfPath = @"wwwroot\temp\orginalform.pdf";
            string outputPdfPath = @"wwwroot\temp\" + Guid.NewGuid().ToString() + ".pdf";





            using PdfReader pdfReader = new(inputPdfPath);
            using PdfWriter pdfWriter = new(outputPdfPath);
            using PdfDocument pdfDocument = new(pdfReader, pdfWriter);
            PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDocument, true);

            // excelden okuma yapılıp Dataset hazırlanacak



            var dataset = await ReadExcel(Guid.Empty);
            var formfilled = await FillForm(form, dataset);


            pdfDocument.Close();

            Console.WriteLine("PDF filled successfully.");
            // return outputPdfPath;


        }


        // Install-Package NPOI

        public async Task<PdfVM> ReadExcel(Guid taskid)
        {
            PdfVM dataset = new();
            string filePath = @"wwwroot\temp\file.xlsx";

            try
            {
                using FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                IWorkbook workbook = new XSSFWorkbook(fs);

                // datasseti doldur
                for (int i = 0; i < workbook.NumberOfSheets; i++)
                {
                    ISheet sheet = workbook.GetSheetAt(i);

                    // Loop through rows and columns to read data
                    for (int rowIndex = 0; rowIndex <= sheet.LastRowNum; rowIndex++)
                    {
                        IRow row = sheet.GetRow(rowIndex);

                        if (row != null)
                        {
                            // Loop through cells in the row
                            foreach (ICell cell in row.Cells)
                            {
                                // dataset te ilgili alanı doldur


                                Console.Write($"{cell}\t");
                            }
                            Console.WriteLine(); // Move to the next row
                        }
                    }

                    Console.WriteLine();
                }



                // dataseti dön
                return dataset;
            }
            catch (Exception)
            {

                throw;
            }
        }




        //// ClosedXML // Install-Package ClosedXML

        //public async Task ReadExcel()
        //{
        //    string filePath = @"wwwroot\temp\file.xlsx";

        //    try
        //    {
        //        using var workbook = new XLWorkbook(filePath);
        //        // Loop through all sheets in the Excel file
        //        foreach (var sheet in workbook.Worksheets)
        //        {
        //            Console.WriteLine($"Reading data from sheet: {sheet.Name}");

        //            // Loop through rows and columns to read data
        //            foreach (var row in sheet.Rows())
        //            {
        //                foreach (var cell in row.Cells())
        //                {
        //                    // Access cell value using cell.Value
        //                    Console.Write($"{cell.Value}\t");
        //                }
        //                Console.WriteLine(); // Move to the next row
        //            }

        //            Console.WriteLine();
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //        throw;
        //    }
        //}


        /// EPPLUS  // Install-Package EPPlus

        // public async Task ReadExcel()
        //{
        //    string filePath = @"wwwroot\temp\file.xlsx";

        //    using var package = new ExcelPackage(new FileInfo(filePath));
        //    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        //    // Loop through all sheets in the Excel file
        //    foreach (var sheet in package.Workbook.Worksheets)
        //    {
        //        Console.WriteLine($"Reading data from sheet: {sheet.Name}");

        //        // Get the dimensions of the sheet
        //        int rowCount = sheet.Dimension.Rows;
        //        int colCount = sheet.Dimension.Columns;

        //        // Loop through rows and columns to read data
        //        for (int row = 1; row <= rowCount; row++)
        //        {
        //            for (int col = 1; col <= colCount; col++)
        //            {
        //                // Access cell value using sheet.Cells[row, col].Value
        //                object cellValue = sheet.Cells[row, col].Value;
        //                Console.Write($"{cellValue}\t");
        //            }
        //            Console.WriteLine(); // Move to the next row
        //        }

        //        Console.WriteLine();
        //    }
        //}

    }
}
