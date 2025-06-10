using System;
using System.Collections.Generic;
using System.Text;
using Codeshell.Abp.Lookups;

namespace Codeshell.Abp.Lookups
{
    public interface INameDescription<T> : INamed<T>
    {
        string Description { get; set; }
    }
}
