namespace PdfFillerApp.Models
{
    public class ConvertJob : BaseModel
    {
        public string Title { get; set; }
        public string? FileVorlageXls { get; set; }
        public string? FileAnlageXls { get; set; }
        public string? ResultPdf { get; set; }   
       
        public Status Status { get; set; }

    }

    public enum Status
    {
        Started, InProcess, Completed, Error
    }

    public class ConvertJobVM:BaseModel
    {
        public string Title { get; set; }
        public IFormFile? FileVorlage { get; set; }
        public IFormFile? FileAnlage { get; set; }
    }
}
