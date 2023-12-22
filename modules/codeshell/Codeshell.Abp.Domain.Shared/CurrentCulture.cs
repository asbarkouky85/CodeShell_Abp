using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Codeshell.Abp
{
    public class CurrentCulture
    {
        public static string Lang => _getCurrent().Name;

        static IScopedProviderAccessor _scopedAccessor;

        static CurrentCulture _getCurrent()
        {
            if (_scopedAccessor == null)
                _scopedAccessor = CodeshellRoot.RootProvider.GetRequiredService<IScopedProviderAccessor>();
            return _scopedAccessor.Provider.GetRequiredService<CurrentCulture>();
        }

        public string Name { get; set; }
    }
}
