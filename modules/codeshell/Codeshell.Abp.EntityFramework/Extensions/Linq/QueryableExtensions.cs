using Codeshell.Abp.Linq;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Codeshell.Abp.Extensions.Linq
{
    public static class QueryableExtensions
    {
        public static async Task<PagedResult<TEntity>> ToPagedResult<TEntity>(
            this IQueryable<TEntity> query,
            ICodeshellPagedRequest requestDto,
            Func<string, Expression<Func<TEntity, bool>>> searchTermFilter = null)
           where TEntity : class
        {
            if (searchTermFilter != null && !string.IsNullOrEmpty(requestDto.Filter))
            {
                var exp = searchTermFilter.Invoke(requestDto.Filter);
                query = query.Where(exp);
            }

            return new PagedResult<TEntity>
            {
                TotalCount = await query.CountAsync(),
                Items = await query.PageWith(requestDto).ToListAsync()
            };
        }

        public static async Task<PagedResult<TEntity>> ToPagedResultForType<TEntity>(
            this IQueryable<TEntity> query,
            CodeshellPagedRequest<TEntity> requestDto,
            Func<string, Expression<Func<TEntity, bool>>> searchTermFilter = null)
           where TEntity : class
        {
            if (requestDto.PropertyFilters.Any())
            {
                foreach (var filter in requestDto.PropertyFilters)
                {
                    query = query.Where(filter.GetExpression<TEntity>());
                }
            }
            return await query.ToPagedResult(requestDto, searchTermFilter);
        }
    }
}
