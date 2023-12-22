using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Domain.Entities;

namespace Codeshell.Abp.Contracts
{
    public interface IRecursiveEntity<TPrime> : IEntity<TPrime> where TPrime : struct
    {
        TPrime? ParentId { get; }
        string Path { get; }
        int? Level { get; }
    }
}
