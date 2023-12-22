using Codeshell.Abp.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;

namespace Codeshell.Abp.Linq.Unique
{
    public static class CheckUniqueExtension
    {
        public static Task<bool> CheckUnique<T, TPrime>(this IQueryable<T> q, CheckUniqueDto<TPrime> dto) where T : class, IEntity<TPrime>
        {

            var ex = Expressions.Unique<T, TPrime>(dto.EntityId, dto.Property, dto.Value);
            var res = !q.Any(ex);
            return Task.FromResult(res);
        }
    }
}
