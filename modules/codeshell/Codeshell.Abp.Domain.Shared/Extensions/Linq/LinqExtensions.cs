using Codeshell.Abp.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace Codeshell.Abp.Extensions.Linq
{
    public static class LinqExtensions
    {
        
        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> lst, Action<T> func)
        {
            foreach (T item in lst)
                func(item);
            return lst;
        }

        public static List<T> MapTo<T>(this IEnumerable lst, bool ignoreId = true, IEnumerable<string> ignore = null) where T : class
        {
            List<T> t = new List<T>();
            foreach (var ob in lst)
            {
                t.Add(ob.MapTo<T>(ignoreId, ignore));
            }
            return t;
        }

        public static T MapTo<T>(this object obj, bool ignoreId = true, IEnumerable<string> ignore = null) where T : class
        {
            T inst = Activator.CreateInstance<T>();
            inst.AppendProperties(obj, ignoreId, ignore);
            return inst;
        }

        public static Expression<Func<T, bool>> Combine<T>(this Expression<Func<T, bool>> exp, Expression<Func<T, bool>> exp2)
        {
            ParameterExpression para = Expression.Parameter(typeof(T));
            BinaryExpression combinedExpression = Expression.MakeBinary(ExpressionType.And, exp, exp2);
            return Expression.Lambda<Func<T, bool>>(combinedExpression, para);
        }



        public static IQueryable<T> PageWith<T>(this IQueryable<T> q, ICodeshellPagedRequest req) where T : class
        {
            if (!string.IsNullOrEmpty(req.Sorting))
            {
                q = q.SortWith(req.Sorting, req.Direction ?? SortDir.DESC);
            }
            if (req.MaxResultCount > 0)
            {
                q = q.Skip(req.SkipCount).Take(req.MaxResultCount);
            }
            return q;
        }


        public static IQueryable<T> PageWith<T>(this IQueryable<T> q, ICodeshellPagedRequest req, Func<string, Expression<Func<T, bool>>> stringFilter) where T : class
        {
            if (!string.IsNullOrEmpty(req.Filter))
            {
                var ex = stringFilter(req.Filter);
                q = q.Where(ex);
            }
            return q.PageWith(req);
        }

        public static IQueryable<T> SortWith<T, TVal>(this IQueryable<T> q, Expression<Func<T, TVal>> exp, SortDir dir) where T : class
        {
            return dir == SortDir.ASC ? q.OrderBy(exp) : q.OrderByDescending(exp);
        }

        public static IQueryable<T> SortWith<T>(this IQueryable<T> q, string propertyName, SortDir dir = SortDir.ASC) where T : class
        {
            PropertyInfo prop = typeof(T).GetProperty(propertyName);

            if (prop == null)
                return q;

            if (prop.PropertyType == typeof(string))
            {
                var exp = Expressions.PropertyExpression<T, string>(propertyName);
                q = q.SortWith(exp, dir);
            }
            else if (prop.PropertyType == typeof(double))
            {
                var exp = Expressions.PropertyExpression<T, double>(propertyName);
                q = q.SortWith(exp, dir);
            }
            else if (prop.PropertyType == typeof(double?))
            {
                var exp = Expressions.PropertyExpression<T, double?>(propertyName);
                q = q.SortWith(exp, dir);
            }
            else if (prop.PropertyType.IsDecimalType())
            {
                var exp = Expressions.PropertyExpression<T, decimal>(propertyName);
                q = q.SortWith(exp, dir);
            }
            else if (prop.PropertyType.IsDecimalType(true))
            {
                var exp = Expressions.PropertyExpression<T, decimal?>(propertyName);
                q = q.SortWith(exp, dir);
            }
            else if (prop.PropertyType.IsIntgerType())
            {
                var exp = Expressions.PropertyExpression<T, long>(propertyName);
                q = q.SortWith(exp, dir);
            }
            else if (prop.PropertyType.IsIntgerType(true))
            {
                var exp = Expressions.PropertyExpression<T, long?>(propertyName);
                q = q.SortWith(exp, dir);
            }
            else if (prop.PropertyType == typeof(DateTime))
            {
                var exp = Expressions.PropertyExpression<T, DateTime>(propertyName);
                q = q.SortWith(exp, dir);
            }
            else if (prop.PropertyType == typeof(DateTime?))
            {
                var exp = Expressions.PropertyExpression<T, DateTime?>(propertyName);
                q = q.SortWith(exp, dir);
            }
            else
            {
                var exp = Expressions.Property<T>(propertyName);
                q = q.SortWith(exp, dir);
            }
            return q;
        }
    }
}
