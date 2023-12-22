using Codeshell.Abp.Cli;
using Codeshell.Abp.Cli.Arguments;
using Codeshell.Abp.Cli.Routing;
using System;
using System.Threading.Tasks;

namespace Codeshell.Abp.Cli.Help
{
    public class HelpCliHandler : CliHandler<HelpRequest>
    {
        public override string FunctionDescription => "Displays documentation for functions in module";
        public HelpCliHandler()
        {
        }

        protected override void Build(ICliRequestBuilder<HelpRequest> builder)
        {

        }

        protected override Task HandleAsync(HelpRequest request)
        {
            var build = GetService<ICliRouteBuilder>();
            foreach (var item in build.HandlerDictionary)
            {
                if (item.Value == GetType())
                    continue;
                ICliHandler? handler = (ICliHandler?)LazyServiceProvider.LazyGetRequiredService(item.Value);

                if (handler != null)
                {
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

            }
            return Task.CompletedTask;
        }
    }
}
