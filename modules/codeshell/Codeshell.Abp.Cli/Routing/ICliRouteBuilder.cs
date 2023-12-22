using Codeshell.Abp.Cli;
using System;
using System.Collections.Generic;
using Volo.Abp.DependencyInjection;

namespace Codeshell.Abp.Cli.Routing
{
    public interface ICliRouteBuilder : ISingletonDependency
    {
        Dictionary<string, Type> HandlerDictionary { get; }
        void AddHandler<T>(string functionName) where T : class, ICliHandler;
        ICliHandler? GetHandler(string func, IServiceProvider prov);
        IEnumerable<Type> HandlerTypes { get; }
    }
}