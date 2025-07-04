using Codeshell.Abp.CliDispatch;
using Codeshell.Abp.ConsoleTest.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Modularity;

namespace Codeshell.Abp.ConsoleTest
{
    [DependsOn(typeof(CodeShellCliDispatchModule))]
    public class CodeshellAbpConsoleTestModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var routes = context.Services.GetCliRouteBuilder();
            routes.AddHandler<TestRequestHandler>("test");
        }
    }

    [DependsOn(typeof(CodeShellCliDispatchModule))]
    public class CodeshellAbpConsoleTest2Module : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var routes = context.Services.GetCliRouteBuilder();
        }
    }
}
