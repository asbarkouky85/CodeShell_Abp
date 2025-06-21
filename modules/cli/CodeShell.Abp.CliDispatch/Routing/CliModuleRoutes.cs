using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Modularity;

namespace Codeshell.Abp.CliDispatch.Routing
{
    public class CliModuleRoutes
    {
        private Dictionary<string, Type> _modules = new Dictionary<string, Type>();

        public void AddModule<TModule>(string key) where TModule : AbpModule
        {
            _modules.Add(key, typeof(TModule));
        }

        public bool IsModule(string key)
        {
            return _modules.ContainsKey(key);
        }

        public Type GetModule(string module)
        {
            if (_modules.TryGetValue(module, out var type))
            {
                return type;
            }
            else
            {
                throw new Exception($"Unknown module '{module}'");
            }
        }
    }
}
