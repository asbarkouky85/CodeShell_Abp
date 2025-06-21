using System;
using System.Collections.Generic;
using System.Text;

namespace Codeshell.Abp.Importation.Excel
{
    public class ExcelSheetConfig
    {
        public IEnumerable<int> DateTimeCols { get; set; } = new int[0];
        public IEnumerable<int> DecimalCols { get; set; } = new int[0];
        public IEnumerable<int> LongCols { get; set; } = new int[0];

    }
}
