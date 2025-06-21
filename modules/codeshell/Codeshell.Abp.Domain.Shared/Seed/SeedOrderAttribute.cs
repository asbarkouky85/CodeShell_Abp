using System;

namespace Codeshell.Abp.Seed
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
