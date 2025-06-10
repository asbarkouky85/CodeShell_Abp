using System;
using System.Collections.Generic;
using System.Text;
using Codeshell.Abp.Lookups;

namespace Codeshell.Abp.Lookups
{
    public interface INamed
    {
        string Name { get; set; }
    }
    public interface INamed<T> : INamed
    {
        T Id { get; }
    }
}
