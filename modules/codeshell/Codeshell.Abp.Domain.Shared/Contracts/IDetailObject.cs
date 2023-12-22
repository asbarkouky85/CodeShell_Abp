using System;
using System.Collections.Generic;
using System.Text;

namespace Codeshell.Abp.Contracts
{

    public interface IDetailObject
    {
        ChangeType ChangeType { get; set; }
    }

    public interface IDetailObject<out T> : IDetailObject
    {
        T Id { get; }
    }
}
