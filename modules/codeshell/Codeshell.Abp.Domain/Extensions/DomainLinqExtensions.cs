using Codeshell.Abp.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.PermissionManagement;

namespace Codeshell.Abp.Extensions.Linq
{
    public static class DomainLinqExtensions
    {
        public static Task GrantFullPermission<T>(this IPermissionDataSeeder seeder, T role, Type permission, Guid? tenantId = null) where T : struct
        {
            return seeder.SeedAsync("R", role.ToString(),
                Utils.Permissions(permission),
                tenantId);
        }

        public static Task GrantPermissions<T>(this IPermissionDataSeeder seeder, T role, IEnumerable<string> perms, Guid? tenantId) where T : struct
        {
            return seeder.SeedAsync("R", role.ToString(),
                perms,
                tenantId);
        }

        public static Task GrantPermissionExcept<T>(this IPermissionDataSeeder seeder, T role, Type permission, IEnumerable<string> exclude, Guid? tenantId = null)
        {
            return seeder.SeedAsync("R", role.ToString(),
                Utils.Permissions(permission, exclude),
                tenantId);
        }

        public static Task<PagedResult<TEntity>> ToPagedResult<TEntity>(
           this IQueryable<TEntity> query, ICodeshellPagedRequest requestDto,
           Func<string, Expression<Func<TEntity, bool>>> searchTermFilter = null)
           where TEntity : class
        {
            if (searchTermFilter != null && !string.IsNullOrEmpty(requestDto.Filter))
            {
                var exp = searchTermFilter.Invoke(requestDto.Filter);
                query = query.Where(exp);
            }
            var data = new PagedResult<TEntity>
            {
                TotalCount = query.Count(),
                Items = query.PageWith(requestDto).ToList()
            };
            return Task.FromResult(data);

        }
    }
}
