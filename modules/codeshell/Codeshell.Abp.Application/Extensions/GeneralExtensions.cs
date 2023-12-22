using Codeshell.Abp.Extensions;
using Codeshell.Abp.Recursion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Volo.Abp.Users;

namespace Codeshell.Abp.Extensions
{
    public static class GeneralExtensions
    {
        public static List<T> RecurseLong<T>(this IEnumerable<T> lst, long? parentId = null)
            where T : class, IRecursiveDto<long, T>
        {
            var main = lst.Where(e => e.ParentId == parentId).ToList();
            foreach (var item in main)
            {
                item.Children = lst.RecurseLong(item.Id);
            }
            return main;
        }

        
    }
}
