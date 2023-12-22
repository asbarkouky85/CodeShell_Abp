using System;
using System.Collections.Generic;
using System.Text;

namespace Codeshell.Abp.Linq
{
    public class CodeshellPagedRequest : ICodeshellPagedRequest
    {
        public string Sorting { get; set; }
        public int SkipCount { get; set; }
        public int MaxResultCount { get; set; }
        public string Filter { get; set; }
        public SortDir? Direction { get; set; }
    }
}
