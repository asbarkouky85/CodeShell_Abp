using System;
using System.Collections.Generic;
using System.Text;
using Codeshell.Abp.Lookups;

namespace Codeshell.Abp.Lookups
{
    public class DescribedDto<TPrime> : Named<TPrime>
    {
        public string Description { get; set; }
    }
}
