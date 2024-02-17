namespace PdfFillerApp.Models
{
    public class SheetDataVM
    {
        public string SheetName { get; set; }
        public List<CellData> CellValues { get; set; }=new List<CellData>();

    }



    public class CellData
    {
        public string Address { get; set; }
        public object Value { get; set; }
    }
}
