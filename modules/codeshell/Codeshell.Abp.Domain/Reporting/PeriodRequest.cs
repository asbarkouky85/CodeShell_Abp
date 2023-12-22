using System;
using System.Collections.Generic;
using System.Text;

namespace Codeshell.Abp.Reporting
{
    public class PeriodRequest
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? Month { get; set; }
        public int? Year { get; set; }

        public DateTime GetStartDate()
        {
            if (StartDate != null)
            {
                return StartDate.Value;
            }

            if (Year != null && Month != null)
            {
                return new DateTime(Year.Value, Month.Value, 1);
            }
            else if (Year != null)
            {
                return new DateTime(Year.Value, 1, 1);
            }
            else if (Month != null)
            {
                return new DateTime(DateTime.Now.Year, Month.Value, 1);
            }
            return DateTime.MinValue;
        }

        public DateTime GetEndDate()
        {
            if (EndDate != null)
            {
                return EndDate.Value;
            }

            if (Year != null && Month != null)
            {
                if (Month == 12)
                {
                    return new DateTime(Year.Value + 1, 1, 1);
                }
                return new DateTime(Year.Value, Month.Value + 1, 1);
            }
            else if (Year != null)
            {
                return new DateTime(Year.Value + 1, 1, 1);
            }
            else if (Month != null)
            {
                if (Month == 12)
                {
                    return new DateTime(DateTime.Now.Year + 1, 1, 1);
                }
                return new DateTime(DateTime.Now.Year, Month.Value + 1, 1);
            }
            return DateTime.Now;
        }
    }
}
