using System;
using System.Collections.Generic;
using System.Text;

namespace Codeshell.Abp.Linq
{
    public interface ICodeshellPagedRequest
    {
        string Sorting { get; set; }
        int SkipCount { get; set; }
        int MaxResultCount { get; set; }
        string Filter { get; set; }
        SortDir? Direction { get; set; }
    }
}
