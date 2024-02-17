using System.ComponentModel.DataAnnotations;

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
        [Display(Name ="Görev Adı")]
        public string Title { get; set; }
        [Display(Name = "Dosya Vorlage")]
        public IFormFile? FileVorlage { get; set; }
        [Display(Name = "Dosya Anlage")]
        public IFormFile? FileAnlage { get; set; }
    }
}
