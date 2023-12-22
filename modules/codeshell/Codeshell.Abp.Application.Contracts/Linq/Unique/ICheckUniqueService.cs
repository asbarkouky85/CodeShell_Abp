using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Codeshell.Abp.Linq.Unique
{
    public interface ICheckUniqueService<TPrime>
    {
        Task<bool> CheckIsUnique(CheckUniqueDto<TPrime> dto);
    }
}
