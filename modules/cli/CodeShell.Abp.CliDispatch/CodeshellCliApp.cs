using Codeshell.Abp.Cli;
using Codeshell.Abp.CliDispatch.Hosting;
using Codeshell.Abp.CliDispatch.Routing;
using Codeshell.Abp.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Volo.Abp.Modularity;

namespace Codeshell.Abp.CliDispatch
{
    public static class CodeshellCliApp
    {
        public static async Task RunAsync<TModule>(string[] args, Action<CliModuleRoutes> moduleRegister = null) where TModule : AbpModule
        {
            var entry = Assembly.GetEntryAssembly();
            Console.WriteLine("---------------------------------------------------------");
            Console.WriteLine($"{entry?.GetName().Name} [" + entry.GetVersionString() + "]");
            _InitLoggers();
            var routes = new CliModuleRoutes();
            routes.AddModule<TModule>("_default");
            moduleRegister?.Invoke(routes);
            var module = _checkEnvironmentAndGetModule(routes, ref args);
            await CreateHostBuilder(module, args, routes).RunConsoleAsync();
        }

        static void _InitLoggers()
        {
            var loggerConfiguration = new LoggerConfiguration()
                .MinimumLevel.Error()
                .WriteTo.Console()
                .WriteTo.Async(e => e.File("Logs/logs.txt"));
            Log.Logger = loggerConfiguration.CreateLogger();
        }

        static string _checkEnvironmentAndGetModule(CliModuleRoutes routes, ref string[] args)
        {
            List<string> useArgs = new List<string>();
            var env = "local";
            string module = null;
            foreach (var arg in args)
            {
                if (module == null)
                {
                    if (routes.IsModule(arg))
                    {
                        module = arg;
                    }
                    else
                    {
                        module = "_default";
                        useArgs.Add(arg);
                    }
                }
                else if (arg.StartsWith("env="))
                {
                    env = arg.Replace("env=", "");
                }
                else
                {
                    useArgs.Add(arg);
                }
            }
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", env);
            Console.Write($"Using Environment [");
            using (ColorSetter.Set(ConsoleColor.Yellow))
            {
                Console.WriteLine(env + "]");
            }

            args = useArgs.ToArray();
            return module ?? "_default";
        }

        public static IHostBuilder CreateHostBuilder(string module, string[] args, CliModuleRoutes routes) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureUsingAppsettings()
                .ConfigureLogging((context, logging) => logging.ClearProviders())
                .ConfigureServices((hostContext, services) =>
                {
                    Type moduleType = routes.GetModule(module);
                    services.AddSingleton(new ConsoleArgumentStore(module, moduleType, args));
                    services.AddHostedService<CliHostedService>();
                });
    }
}
