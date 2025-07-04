using Codeshell.Abp.CliDispatch;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Threading;

namespace Codeshell.Abp.ConsoleTest
{
    public class Program
    {
        static void Main(string[] args)
        {
            AsyncHelper.RunSync(async () =>
            {
                if (Debugger.IsAttached)
                {
                    args = new string[] { "test2", "help", "env=soso" };
                }

                await CodeshellCliApp.RunAsync<CodeshellAbpConsoleTestModule>(args, mods =>
                {
                    mods.AddModule<CodeshellAbpConsoleTest2Module>("test2");
                });

            });
        }
    }
}
