using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace Codeshell.Abp.Recursion
{
    public interface IRecursiveDto<TPrime, T> : IEntityDto<TPrime>
        where T : class, IRecursiveDto<TPrime, T>
        where TPrime : struct
    {
        string Name { get; set; }
        TPrime? ParentId { get; set; }
        bool HasChildren { get; set; }
        List<T> Children { get; set; }
    }
}
