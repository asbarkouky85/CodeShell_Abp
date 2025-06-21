using System;
using System.Collections.Generic;
using System.Text;

namespace Codeshell.Abp.Cli
{
    public interface IOutputWriter
    {
        void WriteLine(bool replaceLast = false);
        void Write(string v, bool replaceLast = false);
        IDisposable Set(ConsoleColor yellow);
        void WriteLine(string v, bool replaceLast = false);
        void GotoColumn(int column);
        
    }
}
