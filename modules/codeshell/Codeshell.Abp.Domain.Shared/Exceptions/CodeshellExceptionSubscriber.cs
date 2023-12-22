using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.ExceptionHandling;
using Codeshell.Abp.Extensions;
using Microsoft.Extensions.Localization;
using Serilog.Core;
using Serilog;

namespace Codeshell.Abp.Exceptions
{
    public class CodeshellExceptionSubscriber : ExceptionSubscriber
    {
        
        private readonly IStringLocalizerFactory stringLocalizerFactory;

        public CodeshellExceptionSubscriber(IStringLocalizerFactory stringLocalizerFactory)
        {
            this.stringLocalizerFactory = stringLocalizerFactory;
        }
        public override Task HandleAsync(ExceptionNotificationContext context)
        {
            
            
            string msg = context.Exception.GetMessageRecursive();
            context.Exception.Data["Message"] = msg;
            
            Log.Error(msg, context.Exception);
            return Task.CompletedTask;
        }
    }
}
