using System.Collections.Generic;

namespace Codeshell.Abp.Linq
{
    public interface IPropertyFilter
    {
        string FilterType { get; set; }
        IEnumerable<string> Ids { get; set; }
        string MemberName { get; set; }
        string Value1 { get; set; }
        string Value2 { get; set; }
    }
}
