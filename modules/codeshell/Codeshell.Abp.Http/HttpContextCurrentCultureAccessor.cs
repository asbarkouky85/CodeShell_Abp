using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Codeshell.Abp
{
    public class HttpContextScopedProviderAccessor : IScopedProviderAccessor
    {

        public HttpContextScopedProviderAccessor()
        {
        }


        public IServiceProvider Provider => _getCurrent();

        IServiceProvider _getCurrent()
        {
            var acc = CodeshellRoot.RootProvider.GetService<IHttpContextAccessor>();
            var prov = acc?.HttpContext?.RequestServices;
            if (prov == null)
            {
                return CodeshellRoot.RootProvider;
            }
            return prov;
        }
    }
}
