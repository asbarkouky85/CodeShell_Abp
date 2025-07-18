﻿using Codeshell.Abp.Cli;
using Codeshell.Abp.CliDispatch.Parsing;
using Codeshell.Abp.Results;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Codeshell.Abp.CliDispatch.Routing
{
    public abstract class CliRequestHandler<T> : ConsoleService, ICliRequestHandler where T : class
    {
        public abstract string FunctionDescription { get; }
        public virtual bool RunInBackground => false;
        protected Dictionary<string, string> ExtraArgs { get; set; } = new Dictionary<string, string>();


        protected CliRequestHandler()
        {

        }

        protected TService GetService<TService>() where TService : class
        {
            return LazyServiceProvider.LazyGetService<TService>();
        }

        protected object GetService(Type t)
        {
            return LazyServiceProvider.LazyGetService(t);
        }

        protected abstract void Build(ICliRequestBuilder<T> builder);
        protected abstract Task<Result> HandleAsync(T request, CancellationToken token);

        public virtual T GetRequestData(string[] args)
        {
            var parser = new CliArgumentParser<T>();
            Build(parser.Builder);
            return parser.Parse(args);
        }

        public virtual async Task<Result> HandleAsync(string[] args, CancellationToken token)
        {

            var req = GetRequestData(args);
            if (req == null)
            {
                return new Result(1);
            }
            return await HandleAsync(req, token);
        }

        public void Document()
        {
            var parser = new CliArgumentParser<T>();
            Build(parser.Builder);
            parser.Builder.Document();
        }

        public void AddArgs(Dictionary<string, string> args)
        {
            foreach (var arg in args)
            {
                ExtraArgs[arg.Key] = arg.Value;
            }
        }

    }
}
