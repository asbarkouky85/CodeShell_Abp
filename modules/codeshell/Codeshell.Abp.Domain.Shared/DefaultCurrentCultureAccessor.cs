using System;
using System.Collections.Generic;
using System.Text;

namespace Codeshell.Abp
{
    public class DefaultScopedProviderAccessor : IScopedProviderAccessor
    {

        public IServiceProvider Provider => CodeshellRoot.RootProvider;

        public DefaultScopedProviderAccessor()
        {

        }
    }
}
