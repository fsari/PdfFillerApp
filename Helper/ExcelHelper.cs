using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace PdfFillerApp.Helper
{
    public static class ExcelHelper
    {
        public static void ConvertXlsToXlsx(string inputPath, string outputPath)
        {
            using (FileStream xlsStream = new(inputPath, FileMode.Open, FileAccess.Read))
            {
                HSSFWorkbook hssfWorkbook = new(xlsStream);

                // Create a new XSSFWorkbook (XLSX)
                XSSFWorkbook xssfWorkbook = new();

                // Loop through each sheet in the XLS workbook and copy it to the XLSX workbook
                for (int i = 0; i < hssfWorkbook.NumberOfSheets; i++)
                {
                    XSSFSheet xssfSheet = (XSSFSheet)xssfWorkbook.CreateSheet(hssfWorkbook.GetSheetName(i));

                    // Copy each row
                    for (int j = 0; j <= hssfWorkbook.GetSheetAt(i).LastRowNum; j++)
                    {
                        IRow row = hssfWorkbook.GetSheetAt(i).GetRow(j);
                        XSSFRow xssfRow = (XSSFRow)xssfSheet.CreateRow(j);

                        if (row != null)
                        {
                            // Copy each cell
                            foreach (ICell cell in row.Cells)
                            {
                                XSSFCell xssfCell = (XSSFCell)xssfRow.CreateCell(cell.ColumnIndex);
                                xssfCell.CellStyle = (XSSFCellStyle)cell.CellStyle;

                                switch (cell.CellType)
                                {
                                    case CellType.Boolean:
                                        xssfCell.SetCellValue(cell.BooleanCellValue);
                                        break;
                                    case CellType.Numeric:
                                        xssfCell.SetCellValue(cell.NumericCellValue);
                                        break;
                                    case CellType.String:
                                        xssfCell.SetCellValue(cell.StringCellValue);
                                        break;
                                    case CellType.Formula:
                                        xssfCell.SetCellFormula(cell.CellFormula);
                                        break;
                                        // Handle other cell types as needed
                                }
                            }
                        }
                    }
                }

                // Save the XLSX workbook
                using FileStream xlsxStream = new(outputPath, FileMode.Create, FileAccess.Write);
                xssfWorkbook.Write(xlsxStream);
            }
        }
    }
}
