using Codeshell.Abp.Cli.Arguments;
using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Codeshell.Abp.Cli
{
    public abstract class CliHandler<T> : ApplicationService, ICliHandler where T : class
    {
        public abstract string FunctionDescription { get; }

        protected CliHandler()
        {
        }

        protected TService GetService<TService>() where TService : class
        {
            return LazyServiceProvider.LazyGetRequiredService<TService>();
        }

        protected object GetService(Type t)
        {
            return LazyServiceProvider.LazyGetRequiredService(t);
        }

        protected abstract void Build(ICliRequestBuilder<T> builder);
        protected abstract Task HandleAsync(T request);

        public virtual T GetRequestData(string[] args)
        {
            var parser = new CliArgumentParser<T>();
            Build(parser.Builder);
            return parser.Parse(args);
        }

        public virtual Task HandleAsync(string[] args)
        {
            var req = GetRequestData(args);
            if (req == null)
            {
                return Task.CompletedTask;
            }

            return HandleAsync(req);
        }

        public void Document()
        {
            var parser = new CliArgumentParser<T>();
            Build(parser.Builder);
            parser.Builder.Document();
        }
    }
}
