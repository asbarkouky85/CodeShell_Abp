using System;
using System.Collections.Generic;
using System.Text;
using Codeshell.Abp.Lookups;

namespace Codeshell.Abp.Lookups
{
    public class NamedShortDto<T> : Named<T> 
    {
        public string ShortName { get; set; }
    }
}
