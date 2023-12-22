using Codeshell.Abp.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.Mvc;

namespace Codeshell.Abp.Http.Controllers
{
    public abstract class HomeControllerBase<TModule> : AbpController
    {

        public HomeControllerBase()
        {

        }

        public virtual ActionResult Index()
        {
            var env = LazyServiceProvider.LazyGetService<IWebHostEnvironment>();
            if (env.IsCodeshellDevelopment())
            {
                return Redirect("/swagger");
            }
            else
            {
                return this.GetAssemblyInfo();
            }
        }

        public ServiceInfoDto Info()
        {
            return LazyServiceProvider.LazyGetService<ICodeShellHealthCheckService>().Info<TModule>();
        }

        public Task<ServiceInfoDto> CheckRemote()
        {
            return LazyServiceProvider.LazyGetService<ICodeShellHealthCheckService>().CheckRemote();
        }


    }
}
