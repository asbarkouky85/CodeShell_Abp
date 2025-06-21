using Codeshell.Abp.CliDispatch.Parsing;
using Codeshell.Abp.CliDispatch.Routing;
using Codeshell.Abp.Results;
using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Codeshell.Abp.Extensions;
using Codeshell.Abp.Cli;

namespace Codeshell.Abp.CliDispatch.Help
{
    public class HelpRequestHandler : CliRequestHandler<HelpRequest>
    {
        public override string FunctionDescription => "";


        protected override void Build(ICliRequestBuilder<HelpRequest> builder)
        {

        }

        protected override Task<Result> HandleAsync(HelpRequest request, CancellationToken t)
        {
            var build = GetService<ICliRouteBuilder>();
            Console.WriteLine();
            Console.WriteLine($"Toolset v{Assembly.GetEntryAssembly().GetVersionString()}");
            Console.WriteLine();
            foreach (var item in build.HandlerDictionary)
            {
                if (item.Value == GetType())
                    continue;
                ICliRequestHandler handler = (ICliRequestHandler)Activator.CreateInstance(item.Value);
                using (ColorSetter.Set(ConsoleColor.Yellow))
                {
                    Console.Write(item.Key);
                }
                using (ColorSetter.Set(ConsoleColor.White))
                {
                    Console.WriteLine(" :\t" + handler.FunctionDescription);
                }

                Console.WriteLine("------------------------------------------------");
                handler.Document();
                Console.WriteLine();
            }
            return Task.FromResult(new Result());
        }
    }
}
