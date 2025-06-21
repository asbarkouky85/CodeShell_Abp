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

        public static CodeshellPagedRequest<T> GetPagedRequestFor<T>(this CodeshellPagedRequest request) where T : class
        {
            var result = new CodeshellPagedRequest<T>
            {
                SkipCount = request.SkipCount,
                Direction = request.Direction,
                Filter = request.Filter,
                MaxResultCount = request.MaxResultCount,
                Sorting = request.Sorting
            };

            if (request.FiltersJson != null)
                result.ReadFilters(request.FiltersJson);

            return result;
        }

        public static Expression<Func<T, bool>> GetExpression<T>(this IPropertyFilter filter) where T : class
        {
            switch (filter.FilterType)
            {
                case "equals":
                    return Expressions.GetEqualsExpression<T>(filter.MemberName, filter.Value1);
                case "string":
                    return Expressions.GetStringContainsFilter<T>(filter.MemberName, filter.Value1);
                case "decimal":
                    return Expressions.GetRangeFilter<T>(filter.MemberName, decimal.Parse(filter.Value1), decimal.Parse(filter.Value2));
                case "int":
                    return Expressions.GetRangeFilter<T>(filter.MemberName, int.Parse(filter.Value1), int.Parse(filter.Value2));
                case "date":
                    var v1 = DateTime.MinValue;
                    var v2 = DateTime.MaxValue;

                    if (DateTime.TryParse(filter.Value1, out DateTime dt))
                        v1 = dt.GetDayStart();

                    if (DateTime.TryParse(filter.Value2, out DateTime dt2))
                        v2 = dt2.GetDayEnd();

                    return Expressions.GetRangeFilter<T>(filter.MemberName, v1, v2);
                case "day":
                    var vd1 = DateTime.MinValue;
                    var vd2 = DateTime.MaxValue;

                    if (DateTime.TryParse(filter.Value1, out DateTime dt3))
                    {
                        vd1 = dt3.GetDayStart();
                        vd2 = dt3.GetDayEnd();
                        return Expressions.GetRangeFilter<T>(filter.MemberName, vd1, vd2);
                    }
                    break;
                case "reference":
                    return Expressions.GetReferenceContainedFilter<T>(filter.MemberName, filter.Ids);
            }
            return null;
        }
    }
}
