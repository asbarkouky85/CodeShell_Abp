using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Volo.Abp.Users;

namespace Codeshell.Abp.Extensions
{
    public static class CurrentUserExtensions
    {
        public static bool HasType<T>(this ICurrentUser user, T type) where T : struct
        {
            var t = user.GetUserTypeAs<T>();
            return type.Equals(t);
        }

        public static T? GetUserTypeAs<T>(this ICurrentUser user, string claimKey = CodeshellConstants.UserTypeClaimKey) where T : struct
        {
            var claim = user.FindClaim(claimKey);
            if (claim != null && claim.Value != null && Enum.TryParse<T>(claim.Value, out T type))
            {
                return type;
            }
            return null;
        }

        public static void SetUserType<T>(this ClaimsPrincipal principale, T type, string claimKey = CodeshellConstants.UserTypeClaimKey) where T : struct
        {
            var claim = new Claim(claimKey, type.ToString());
            var frontClaim = new Claim("acr", type.ToString());
            principale.Identities.First().AddClaim(claim);
            principale.Identities.First().AddClaim(frontClaim);
        }

        public static T GetRoleAs<T>(this ICurrentUser user) where T : struct
        {
            foreach (var r in user.Roles)
            {
                if (Enum.TryParse(r, out T enumRole))
                {
                    return enumRole;
                }
            }
            return default;
        }

        public static IEnumerable<T> GetRolesAs<T>(this ICurrentUser user) where T : struct
        {
            List<T> lst = new List<T>();
            foreach (var r in user.Roles)
            {
                if (Enum.TryParse(r, out T enumRole))
                {
                    lst.Add(enumRole);
                }
            }
            return lst;
        }

        public static bool HasRole<T>(this ICurrentUser user, params T[] roles) where T : struct
        {
            foreach (var r in roles)
            {
                if (user.Roles.Any(e => e == r.ToString()))
                    return true;
            }
            return false;
        }

        public static string GetName(this ICurrentUser user)
        {
            if (user.Name == user.SurName)
                return user.Name;
            string name = "";
            if (!string.IsNullOrEmpty(user.Name))
                name += user.Name;
            if (!string.IsNullOrEmpty(user.SurName))
                name += " " + user.SurName;
            return name;
        }
    }
}
