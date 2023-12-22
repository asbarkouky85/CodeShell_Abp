using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.AspNetCore.Mvc.ExceptionHandling;
using Volo.Abp.DependencyInjection;

namespace Codeshell.Abp.Exceptions
{
    [ExposeServices(typeof(AbpExceptionFilter), typeof(IAsyncExceptionFilter))]
    public class CodeshellExceptionFilter : AbpExceptionFilter, ITransientDependency
    {
        private readonly IStringLocalizerFactory stringLocalizerFactory;
        public CodeshellExceptionFilter(IStringLocalizerFactory stringLocalizerFactory)
        {
            this.stringLocalizerFactory = stringLocalizerFactory;
        }

        protected override Task HandleAndWrapException(ExceptionContext context)
        {
            if (context.Exception is LocalizableException)
            {
                var localizableEx = (LocalizableException)context.Exception;
                if (localizableEx.ResourceType != null)
                {
                    localizableEx.SetMessage(stringLocalizerFactory.Create(localizableEx.ResourceType));
                }
            }
            return base.HandleAndWrapException(context);
        }
    }
}
