using System.Collections.Generic;

namespace Codeshell.Abp.Importation.Excel
{
    public class ExcelConfiguration
    {
        public List<ExcelSheetConfig> Sheets { get; set; }
        public IEnumerable<string> DateFormatters { get; set; } = new string[0];
    }
}
