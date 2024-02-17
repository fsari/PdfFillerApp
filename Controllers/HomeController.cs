using iText.Forms;
using iText.Kernel.Pdf;
using Microsoft.AspNetCore.Mvc;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using PdfFillerApp.Data;
using PdfFillerApp.Helper;
using PdfFillerApp.Models;

namespace PdfFillerApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IWebHostEnvironment _env;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, IWebHostEnvironment env, ApplicationDbContext context)
        {
            _logger = logger;
            _env = env;
            _context = context;
        }

        public IActionResult Index()
        {
            var taskList = _context.ConvertJobs.Where(i => !i.IsDeleted).OrderByDescending(i => i.CreatedAt).ToList();
            var list = MinioHelper.ListObjects().GetAwaiter().GetResult();

            return View(taskList);
        }


        public IActionResult NewTask()
        {
            return View(new ConvertJobVM { });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NewTask(ConvertJobVM vm)
        {

            // --------------------- Dosyalar geçerli mi? İkisi de geçerli değilse hata ver -----------------------------

            if (vm.FileAnlage == null || vm.FileAnlage.Length == 0 || vm.FileVorlage == null || vm.FileVorlage.Length == 0)
            {
                return BadRequest("Dosya Geçersiz");
            }

            //-----------------------------  Burada ilk dosya okunuyor ANLAGE   ------------------------------------------
            string fan = vm.FileAnlage.FileName.TrCharsToEngCharsSub200();
            var fileanlage = Path.Combine(@"wwwroot\temp\inputxlsx\", fan);
            using (var stanlage = new FileStream(fileanlage, FileMode.Create))
            {
                vm.FileAnlage.CopyToAsync(stanlage).GetAwaiter().GetResult();
                stanlage.Dispose();
            }
            var resulta = await MinioHelper.Upload(fileanlage, vm.Title, ".xlsx");



            //-----------------------------  Burada ikinci dosya okunuyor VORLAGE   ------------------------------------
            string fvn = vm.FileVorlage.FileName.TrCharsToEngCharsSub200();
            var filevorlage = Path.Combine(@"wwwroot\temp\inputxlsx\", fvn);
            using (var stvorlage = new FileStream(filevorlage, FileMode.Create))
            {
                vm.FileAnlage.CopyToAsync(stvorlage).GetAwaiter().GetResult();
                stvorlage.Dispose();
            }
            var resultv = await MinioHelper.Upload(filevorlage, vm.Title, ".xlsx");


            //--------------------------------- Veritabanına yaz ---------------------------------------

            var cj = new ConvertJob
            {
                Title = vm.Title,
                Status = Status.Started,
                FileAnlageXls = resulta,
                FileVorlageXls = resultv,
                CreatedAt = DateTime.Now,
            };
            _context.ConvertJobs.Add(cj);
            _context.SaveChanges();

            return RedirectToAction("Index");

        }



        /// <summary>
        /// Minio Sunucusunda ki dosyayı indirir. Sonra sunucudan siler
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public async Task<IActionResult> DownloadFile(string filename)
        {
            await MinioHelper.DownloadFile(filename);

            Path.Combine(_env.WebRootPath, filename).DeleteFile();
            return RedirectToAction("Index");
        }



        #region PDF İşlemleri
        /// <summary>
        /// PDF içindeki field ları veri ile doldurur
        /// </summary>
        /// <param name="form"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<PdfAcroForm> FillForm(PdfAcroForm form, PdfVM data)
        {

            // Replace "fieldName" with the actual field name in your PDF
            form.GetField("name").SetValue(data.name);
            form.GetField("vorname").SetValue(data.vorname);
            form.GetField("antragsteller").SetValue(data.antragsteller);
            form.GetField("akademischer_grad").SetValue(data.akademischer_grad);

            return form;
        }


        /// <summary>
        /// PDF doldurur
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task FillPdf(Guid id)
        {
            var cj = _context.ConvertJobs.Find(id);

            string inputPdfPath = @"wwwroot\template\orginalform.pdf";
            string outputPdfPath = @"wwwroot\temp\output\" + cj.Title.TrCharsToEngCharsSub200() + ".pdf";


            using PdfReader pdfReader = new(inputPdfPath);
            using PdfWriter pdfWriter = new(outputPdfPath);
            using PdfDocument pdfDocument = new(pdfReader, pdfWriter);
            PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDocument, true);

            // excelden okuma yapılıp Dataset hazırlanacak
            var cellvaluelist = await ReadExcel(id);


            // TODO: cellValueList ten veriler alınarak pdfvm doldurulacak
            PdfVM pdfVM = new();

            // ???  hücre ve etiket eşlemelerini tutan eşleme tablosu ??? kullanılsın mı yoksa doğrudan vm ypaılsın mı bilemedim??
            var pairingList = _context.CellPdfPairings.ToList();

            foreach (var item in pairingList)
            {
                // eşleştirme tablosu kullanılacak mı????

            }

            // Doldurulan vm pdf için gönderilecek
            var formfilled = await FillForm(form, pdfVM);


            var resulta = await MinioHelper.Upload(outputPdfPath, cj.Title, ".pdf");

            pdfDocument.Close();

        }

        #endregion




        public void UploadFileAndConvert(string sourceFilePath, string destinationFilePath)
        {
            // Check if the file is XLS
            if (Path.GetExtension(sourceFilePath).Equals(".xls", StringComparison.OrdinalIgnoreCase))
            {
                ExcelHelper.ConvertXlsToXlsx(sourceFilePath, destinationFilePath);
            }

        }

        /// <summary>
        /// Excel dosyalarını Minio sunucusundan temp e indirip okur sonra tempten siler
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<List<SheetDataVM>> ReadExcel(Guid? id)
        {
            List<SheetDataVM> sheetDataList = new();


            var cjob = _context.ConvertJobs.Find(id);

            if (cjob != null)
            {

                MinioHelper.DownloadFile(cjob.FileAnlageXls, true).GetAwaiter().GetResult();
                MinioHelper.DownloadFile(cjob.FileVorlageXls, true).GetAwaiter().GetResult();



                string fileanlage = @"wwwroot\temp\downloaded\" + cjob.FileAnlageXls;
                string filevorlage = @"wwwroot\temp\downloaded\" + cjob.FileVorlageXls;

                try
                {
                    using FileStream fs = new(fileanlage, FileMode.Open, FileAccess.Read);
                    IWorkbook workbook = new XSSFWorkbook(fs);

                    // datasseti doldur
                    for (int i = 0; i < workbook.NumberOfSheets; i++)
                    {

                        ISheet sheet = workbook.GetSheetAt(i);
                        SheetDataVM sheetData = new() { SheetName = sheet.SheetName };

                        // Loop rows / columns
                        for (int rowIndex = 0; rowIndex <= sheet.LastRowNum; rowIndex++)
                        {
                            IRow row = sheet.GetRow(rowIndex);

                            if (row != null)
                            {

                                // Loop cells in the row
                                foreach (ICell cell in row.Cells)
                                {
                                    Console.WriteLine($"{cell.Address} - {cell}");

                                    if (!string.IsNullOrEmpty(cell.ToString()))
                                    {
                                        sheetData.CellValues.Add(new CellData
                                        {
                                            Address = cell.Address.ToString(),
                                            Value = cell
                                        });
                                    }

                                }
                                Console.WriteLine();
                            }
                        }
                        sheetDataList.Add(sheetData);
                    }

                    //----------------

                    using FileStream fsv = new(filevorlage, FileMode.Open, FileAccess.Read);
                    IWorkbook workbookv = new XSSFWorkbook(fsv);

                    // datasseti doldur
                    for (int i = 0; i < workbook.NumberOfSheets; i++)
                    {
                        ISheet sheet = workbookv.GetSheetAt(i);
                        SheetDataVM sheetData = new() { SheetName = sheet.SheetName };

                        // Loop rows / columns
                        for (int rowIndex = 0; rowIndex <= sheet.LastRowNum; rowIndex++)
                        {
                            IRow row = sheet.GetRow(rowIndex);

                            if (row != null)
                            {

                                // Loop cells in the row
                                foreach (ICell cell in row.Cells)
                                {
                                    Console.WriteLine($"{cell.Address} - {cell}");

                                    if (!string.IsNullOrEmpty(cell.ToString()))
                                    {
                                        sheetData.CellValues.Add(new CellData
                                        {
                                            Address = cell.Address.ToString(),
                                            Value = cell
                                        });
                                    }
                                }
                            }
                        }
                        sheetDataList.Add(sheetData);
                    }

                }
                catch (Exception ex)
                {
                    throw;
                }
                finally
                {
                    fileanlage.DeleteFile();
                    filevorlage.DeleteFile();
                }
            }

            return sheetDataList;
        }


    }
}
