using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

using InsanityBot.Utility.Permissions.Reference;

using Newtonsoft.Json;

namespace InsanityBot.Utility.Permissions.Intermediary.Default
{
    public class DefaultPermissions : PermissionBase
    {
        public override UInt64 SnowflakeIdentifier
        {
            get => base.SnowflakeIdentifier;
            set => base.SnowflakeIdentifier = 0; // this is a default, only 0 is allowed here
        }

        public DefaultPermissions Register(PermissionDeclaration[] decl)
        {
            foreach (var v in decl)
                Permissions.TryAdd(v.Permission, v.Default);
            return this;
        }

        public void Serialize()
        {
            StreamWriter writer = new("./config/permissions/default.json");
            writer.BaseStream.SetLength(0);

            writer.Write(JsonConvert.SerializeObject(this));
            writer.Close();
        }

        public static DefaultPermissions Deserialize()
        {
            StreamReader reader = new("./config/permissions/default.json");

            return JsonConvert.DeserializeObject<DefaultPermissions>(reader.ReadToEnd());
        }

        public static DefaultPermissions operator +(DefaultPermissions left, PermissionDeclaration[] right)
        {
            return left.Register(right);
        }

        public static DefaultPermissions operator +(DefaultPermissions left, PermissionDeclaration right)
        {
            DefaultPermissions permissions = left;
            permissions.Permissions.Add(right.Permission, right.Default);
            return permissions;
        }

        // design decision: do we support removing default permissions?
        // probably not, since they are auto-handled by the bot and shouldnt be changed from outside the permission subsystem

        [Obsolete("We do not explicitly support removing the auto-generated default permissions")]
        public static DefaultPermissions operator -(DefaultPermissions left, PermissionDeclaration[] right)
        {
            DefaultPermissions permissions = left;
            foreach (var v in right)
                permissions.Permissions.Remove(v.Permission);
            return permissions;
        }

        [Obsolete("We do not explicitly support removing the auto-generated default permissions")]
        public static DefaultPermissions operator -(DefaultPermissions left, PermissionDeclaration right)
        {
            DefaultPermissions permissions = left;
            permissions.Permissions.Remove(right.Permission);
            return permissions;
        }
    }
}
