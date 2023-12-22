using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codeshell.Abp.Cli.Arguments
{
    public class ConsoleArgumentStore
    {
        public string Module { get; set; }
        public string[] Arguments { get; set; }
        public Type ModuleType { get; set; }
        protected ConsoleArgumentStore()
        {
            Module = "";
            Arguments = new string[0];
            ModuleType = typeof(ConsoleArgumentStore);
        }

        public ConsoleArgumentStore(string module, Type moduleType, string[] args) : this()
        {
            Module = module;
            Arguments = args;
            ModuleType = moduleType;
        }

        public string ModuleCliDisplay => Module == "_default" ? "" : $"[{Module}]";


    }
}
