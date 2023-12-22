using Codeshell.Abp.Cli;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Codeshell.Abp.Cli.Routing
{
    internal class CliRouteBuilder : ICliRouteBuilder
    {
        private Dictionary<string, Type> _handlerList = new Dictionary<string, Type>();

        public IEnumerable<Type> HandlerTypes => _getHandlerTypes();
        public Dictionary<string, Type> HandlerDictionary => _handlerList;

        public void AddHandler<T>(string functionName) where T : class, ICliHandler
        {
            _handlerList[functionName] = typeof(T);
        }

        public ICliHandler? GetHandler(string func, IServiceProvider prov)
        {
            if (_handlerList.TryGetValue(func, out Type? t))
            {
                return (ICliHandler?)prov.GetService(t);
            }
            return null;
        }

        private IReadOnlyList<Type> _getHandlerTypes()
        {
            var lst = _handlerList.Select(e => e.Value).ToList();
            return lst.ToArray();
        }

    }
}
