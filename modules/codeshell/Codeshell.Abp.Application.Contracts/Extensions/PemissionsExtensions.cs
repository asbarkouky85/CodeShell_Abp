using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;
using Volo.Abp.MultiTenancy;

namespace Codeshell.Abp.Extensions
{
    public static class PemissionsExtensions
    {
        public static void AddPermissions<TResource>(
            this IPermissionDefinitionContext definition,
            Type type,
            string groupName = null,
            MultiTenancySides multiTenancySide = MultiTenancySides.Both,
            bool isEnabled = true,
            string[] ignore = null)
        {
            var def = Utils.GetDefaultPermission(type);
            var group = definition.GetGroupOrNull(groupName ?? type.Name);
            if (group == null)
                group = definition.AddGroup(groupName ?? type.Name, LocalizableString.Create<TResource>(groupName ?? type.Name));
            var lst = Utils.Permissions(type, ignore);
            if (def != null)
            {
                var defName = LocalizableString.Create<TResource>(def.GetAfterFirst("."));
                var defaultPermission = group.AddPermission(def, defName, multiTenancySide, isEnabled);
                lst = lst.Where(e => e != def).ToArray();
                foreach (var item in lst)
                {
                    var loc = LocalizableString.Create<TResource>(item.GetAfterLast("."));
                    defaultPermission.AddChild(item, loc, multiTenancySide, isEnabled);
                }
            }
            else
            {
                foreach (var item in lst)
                {
                    var loc = LocalizableString.Create<TResource>(item.GetAfterLast("."));
                    group.AddPermission(item, loc, multiTenancySide, isEnabled);
                }
            }

        }
    }
}
