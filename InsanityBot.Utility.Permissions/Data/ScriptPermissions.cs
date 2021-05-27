using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace InsanityBot.Utility.Permissions.Data
{
    public class ScriptPermissions
    {
        public PermissionDeclaration[] Scripts { get; set; }

        public ScriptPermissions() => Scripts = Array.Empty<PermissionDeclaration>();

        public static ScriptPermissions BuildScriptPermissions()
        {
            IEnumerable<String> v = from v1 in Directory.GetFiles(DefaultPermissionFileSpecifications.Script.Path)
                                    where v1.EndsWith(".is") || v1.EndsWith(".isc")
                                    select v1;

            ScriptPermissions perms = new();

            foreach(String v2 in v)
            {
                StreamReader reader = new(v2);
                String nameHeader = reader.ReadLine();
                String permissionHeader = reader.ReadLine();

                /*
                 * this should look like:
                 * 
                 * HEADER name@version - author
                 * HEADER permission
                */

                String name = nameHeader.Split('#', ' ')[1];
                Boolean permission = Convert.ToBoolean(permissionHeader.Split(' ')[1]);

                perms.Scripts = perms.Scripts.Append(
                    new()
                    {
                        Permission = $"script.{name}",
                        Value = permission
                    })
                    .ToArray();
            }

            return perms;
        }

        public ScriptPermissions UpdateScriptPermissions()
        {
            ScriptPermissions perms = BuildScriptPermissions();

            foreach(PermissionDeclaration v in perms.Scripts)
            {
                if(!this.Scripts.Contains(v))
                {
                    this.Scripts = this.Scripts.Append(v).ToArray();
                }
            }

            return this;
        }
    }
}
