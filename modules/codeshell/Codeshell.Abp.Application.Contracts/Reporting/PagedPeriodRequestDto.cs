using Codeshell.Abp.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Codeshell.Abp.Reporting
{
    public class PagedPeriodRequestDto : CodeshellPagedRequestDto
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? Month { get; set; }
        public int? Year { get; set; }
    }
}
