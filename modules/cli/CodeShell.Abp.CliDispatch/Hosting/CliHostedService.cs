using Codeshell.Abp.Cli;
using Codeshell.Abp.CliDispatch.Routing;
using Codeshell.Abp.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Uow;

namespace Codeshell.Abp.CliDispatch.Hosting
{
    public class CliHostedService : IHostedService
    {
        private readonly ConsoleArgumentStore ArgumentStore;
        private readonly IHostApplicationLifetime _hostApplicationLifetime;
        public CliHostedService(ConsoleArgumentStore aguments, IHostApplicationLifetime hostApplicationLifetime)
        {
            ArgumentStore = aguments;
            _hostApplicationLifetime = hostApplicationLifetime;
        }

        string[] _extractArguments(string argumentString)
        {
            var args = new List<string>();
            var argumentSegments = argumentString.Split(' ');
            string quotedArg = null;
            foreach (var argumentSegment in argumentSegments)
            {
                if (string.IsNullOrEmpty(argumentSegment))
                    continue;

                if (argumentSegment.StartsWith("\""))
                {
                    quotedArg = argumentSegment;
                }
                else if (quotedArg != null)
                {
                    quotedArg += argumentSegment;
                    if (argumentSegment.EndsWith("\""))
                    {
                        args.Add(quotedArg);
                        quotedArg = null;
                    }
                }
                else
                {
                    args.Add(argumentSegment);
                }
            }
            return args.ToArray();
        }

        private string[] _getArguments()
        {

            var arguments = ArgumentStore.Arguments;
            if (!arguments.Any())
            {
                var entryAssembly = Assembly.GetEntryAssembly();

                Console.Write($"{entryAssembly?.GetName().Name?.ToLower()}{ArgumentStore.ModuleCliDisplay}>");
                var argumentString = Console.ReadLine();

                if (!string.IsNullOrEmpty(argumentString))
                {
                    arguments = _extractArguments(argumentString);
                }
            }

            return arguments;
        }

        protected virtual void OnApplicationCreation(AbpApplicationCreationOptions opts)
        {

        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using (var application = AbpApplicationFactory.Create(ArgumentStore.ModuleType, opts =>
            {
                opts.UseAutofac();
                opts.Services.AddLogging(c => c.AddSerilog());
                opts.Configuration.EnvironmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                OnApplicationCreation(opts);
            }))
            {
                Console.Write($"Initializing Module {ArgumentStore.ModuleCliDisplay}... ");
                application.Initialize();
                Console.WriteLine($"Ready");
                Console.WriteLine();

                var excuteOnce = ArgumentStore.Arguments.Any();
                var builder = application.ServiceProvider.GetRequiredService<ICliRouteBuilder>();
                var manager = application.ServiceProvider.GetRequiredService<IUnitOfWorkManager>();

                while (true)
                {
                    var arguments = _getArguments();

                    if (!arguments.Any())
                    {
                        continue;
                    }

                    var command = arguments.First();
                    if (command.ToLower() == "exit")
                    {
                        break;
                    }

                    if (command.ToLower() == "cls")
                    {
                        Console.Clear();
                        continue;
                    }

                    using (var uow = manager.Begin(true, true))
                    {
                        bool isSuccess = false;
                        try
                        {
                            var cliRequestHandler = builder.GetHandler(command, uow.ServiceProvider);

                            if (cliRequestHandler == null)
                            {
                                Console.WriteLine("Unknow function : " + command);
                                if (excuteOnce)
                                    break;
                                else
                                    continue;
                            }

                            Console.WriteLine();
                            Console.WriteLine("Executing Command [" + command + "]..");
                            Console.WriteLine();
                            var cts = new CancellationTokenSource();

                            if (cliRequestHandler.RunInBackground)
                            {
                                var t = cliRequestHandler.HandleAsync(arguments, cts.Token);

                                t.GetAwaiter().OnCompleted(() =>
                                {
                                    isSuccess = true;
                                    cts.Cancel();
                                });

                                WaitForKey(cts.Token);

                                cts.Cancel();
                                cts.Token.ThrowIfCancellationRequested();
                            }
                            else
                            {
                                await cliRequestHandler.HandleAsync(arguments, cts.Token);
                            }

                            if (command != "help")
                            {
                                Console.WriteLine();
                                Console.Write("Handling Complete, Saving to database...");
                                await uow.CompleteAsync();
                                Console.WriteLine("Saving Complete");

                            }
                            Console.WriteLine();
                        }
                        catch (OperationCanceledException)
                        {
                            if (!isSuccess)
                            {
                                Console.WriteLine();
                                Console.WriteLine("Task Cancelled");
                            }
                        }
                        catch (Exception ex)
                        {
                            var writer = uow.ServiceProvider.GetRequiredService<IOutputWriter>();
                            var c = new ConsoleService(writer);
                            c.WriteException(ex);
                        }
                    }

                    if (excuteOnce)
                        break;
                }
                application.Shutdown();
                _hostApplicationLifetime.StopApplication();
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private ConsoleKeyInfo WaitForKey(CancellationToken token)
        {
            int delay = 0;
            while (true)
            {
                if (Console.KeyAvailable)
                {
                    return Console.ReadKey();
                }
                token.ThrowIfCancellationRequested();
                Thread.Sleep(50);
                delay += 50;
            }
        }
    }
}
