using System;
using System.Collections.Generic;
using System.Text;

namespace Codeshell.Abp.Data
{
    public class SeedOrderAttribute : Attribute
    {
        public int Order { get; private set; }
        public SeedOrderAttribute(int order)
        {
            Order = order;
        }
    }
}
