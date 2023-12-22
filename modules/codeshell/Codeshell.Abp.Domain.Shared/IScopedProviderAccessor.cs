using System;
using System.Collections.Generic;
using System.Text;

namespace Codeshell.Abp
{
    public interface IScopedProviderAccessor
    {
        IServiceProvider Provider { get; }
    }
}
