using System.Collections;

namespace Codeshell.Abp.Lookups
{
    public class NamedChildren : Named<object>
    {
        public IEnumerable Children { get; set; }
    }
}
