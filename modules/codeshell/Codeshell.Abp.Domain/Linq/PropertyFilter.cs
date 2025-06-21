﻿using System;
using System.Collections.Generic;
using System.Text;
using Codeshell.Abp.Linq;

namespace Codeshell.Abp.Linq
{
    public class PropertyFilter : IPropertyFilter
    {
        public string MemberName { get; set; }
        public string FilterType { get; set; }
        public string Value1 { get; set; }
        public string Value2 { get; set; }
        public IEnumerable<string> Ids { get; set; }
    }
}
