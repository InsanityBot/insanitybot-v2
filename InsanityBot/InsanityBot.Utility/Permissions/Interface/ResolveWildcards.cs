using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using InsanityBot.Utility.Permissions.Controller;
using InsanityBot.Utility.Permissions.Model;

namespace InsanityBot.Utility.Permissions.Interface
{
    internal static class ResolveWildcards
    {
        internal static List<String> ResolveWildcardPermissions(this String permission)
            => ResolveWildcardPermissions(new String[] { permission });

        internal static List<String> ResolveWildcardPermissions(this String[] permissions)
        {
            List<String> resolved = new();
            DefaultPermissions defaults = DefaultPermissionSerializer.GetDefaultPermissions();

            foreach (var v in permissions)
                if (!v.Contains('*'))
                    resolved.Add(v);

            foreach(var v in permissions)
            {
                if (!v.Contains('*'))
                    continue;

                if (v[v.IndexOf('*') - 1] != '.')
                    throw new ArgumentException("Invalid wildcard.", nameof(permissions));

                resolved = resolved.Concat(
                    from x in defaults.Permissions
                    where x.Key.StartsWith(v.Split('*')[0])
                    select x.Key
                    ).ToList();
            }

            return resolved;
        }
    }
}
