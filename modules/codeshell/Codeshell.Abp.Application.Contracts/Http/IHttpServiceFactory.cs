using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.DependencyInjection;

namespace Codeshell.Abp.Http
{
    public interface IHttpServiceFactory : ITransientDependency
    {
        IHttpService Create(string baseUrl);
    }
}
