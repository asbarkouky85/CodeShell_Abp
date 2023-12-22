using System;
using System.Collections.Generic;
using System.Text;

namespace Codeshell.Abp.Linq.Unique
{
    public class CheckUniqueDto<TPrime>
    {
        public TPrime EntityId { get; set; }
        public string Property { get; set; }
        public string Value { get; set; }
    }
}
