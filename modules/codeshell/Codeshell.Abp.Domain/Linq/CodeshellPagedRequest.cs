using Codeshell.Abp.Extensions;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Codeshell.Abp.Linq
{
    public class CodeshellPagedRequest : ICodeshellPagedRequest
    {
        public string Sorting { get; set; }
        public int SkipCount { get; set; }
        public int MaxResultCount { get; set; }
        public string Filter { get; set; }
        public string FiltersJson { get; set; }
        public SortDir? Direction { get; set; }


    }

    public class CodeshellPagedRequest<T> : ICodeshellPagedRequest
    {
        public string Sorting { get; set; }
        public int SkipCount { get; set; }
        public int MaxResultCount { get; set; }
        public string Filter { get; set; }
        public SortDir? Direction { get; set; }
        public List<PropertyFilter> PropertyFilters { get; private set; } = new List<PropertyFilter>();

        public void ReadFilters(string filtersJson)
        {
            PropertyFilters = filtersJson.FromJson<List<PropertyFilter>>();
        }


    }
}
