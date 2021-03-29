#define INSANITYBOT_DO_NOT_OVERRIDE_ENGINE
//#undef INSANITYBOT_DO_NOT_OVERRIDE_ENGINE
// uncomment this if you want to allow external handlers for permissions.
// this requires a full recompile to apply. the feature is also disabled for a *reason*.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

using DSharpPlus;
using DSharpPlus.Entities;

using InsanityBot.Utility.Permissions.Data;

using Newtonsoft.Json;

namespace InsanityBot.Utility.Permissions
{
    public class PermissionEngine
    {
        #region Delegates
        public delegate void UpdateUserPermissionsDelegate(UInt64 UserId);
        public delegate void UpdateRolePermissionsDelegate(UInt64 RoleId);
        public delegate void GrantUserPermissionsDelegate(UInt64 UserId, String[] Permissions);
        public delegate void GrantRolePermissionsDelegate(UInt64 RoleId, String[] Permissions);
        public delegate void RevokeUserPermissionsDelegate(UInt64 UserId, String[] Permissions);
        public delegate void RevokeRolePermissionsDelegate(UInt64 RoleId, String[] Permissions);
        public delegate void NeutralizeUserPermissionsDelegate(UInt64 UserId, String[] Permissions);
        public delegate void NeutralizeRolePermissionsDelegate(UInt64 RoleId, String[] Permissions);
        public delegate void SetUserAdministratorDelegate(UInt64 UserId, String[] Permissions);
        public delegate void SetRoleAdministratorDelegate(UInt64 RoleId, String[] Permissions);
        public delegate void CreateUserPermissionsDelegate(UInt64 UserId);
        public delegate void CreateRolePermissionsDelegate(UInt64 RoleId);
        public delegate UserPermissions GetUserPermissionsDelegate(UInt64 UserId);
        public delegate RolePermissions GetRolePermissionsDelegate(UInt64 RoleId);
        public delegate void SetUserPermissionsDelegate(UserPermissions User);
        public delegate void SetRolePermissionsDelegate(RolePermissions Role);
        public delegate RolePermissions ApplyMappingsDelegate(DiscordRole Role);
        public delegate void DeleteUserPermissionsDelegate(UInt64 UserId);
        public delegate void DeleteRolePermissionsDelegate(UInt64 UserId);

#if INSANITYBOT_DO_NOT_OVERRIDE_ENGINE
        public UpdateUserPermissionsDelegate UpdateUserPermissions { get; internal set; }
        public UpdateRolePermissionsDelegate UpdateRolePermissions { get; internal set; }
        public GrantUserPermissionsDelegate GrantUserPermissions { get; internal set; }
        public GrantRolePermissionsDelegate GrantRolePermissions { get; internal set; }
        public RevokeUserPermissionsDelegate RevokeUserPermissions { get; internal set; }
        public RevokeRolePermissionsDelegate RevokeRolePermissions { get; internal set; }
        public NeutralizeUserPermissionsDelegate NeutralizeUserPermissions { get; internal set; }
        public NeutralizeRolePermissionsDelegate NeutralizeRolePermissions { get; internal set; }
        public SetUserAdministratorDelegate SetUserAdministrator { get; internal set; }
        public SetRoleAdministratorDelegate SetRoleAdministrator { get; internal set; }
        public CreateUserPermissionsDelegate CreateUserPermissions { get; internal set; }
        public CreateRolePermissionsDelegate CreateRolePermissions { get; internal set; }
        public GetUserPermissionsDelegate GetUserPermissions { get; internal set; }
        public GetRolePermissionsDelegate GetRolePermissions { get; internal set; }
        public SetUserPermissionsDelegate SetUserPermissions { get; internal set; }
        public SetRolePermissionsDelegate SetRolePermissions { get; internal set; }
        public ApplyMappingsDelegate ApplyMappings { get; internal set; }
        public DeleteUserPermissionsDelegate DeleteUserPermissions { get; internal set; }
        public DeleteRolePermissionsDelegate DeleteRolePermissions { get; internal set; }
#else
        public UpdateUserPermissionsDelegate UpdateUserPermissions { get; set; }
        public UpdateRolePermissionsDelegate UpdateRolePermissions { get; set; }
        public GrantUserPermissionsDelegate GrantUserPermissions { get; set; }
        public GrantRolePermissionsDelegate GrantRolePermissions { get; set; }
        public RevokeUserPermissionsDelegate RevokeUserPermissions { get; set; }
        public RevokeRolePermissionsDelegate RevokeRolePermissions { get; set; }
        public NeutralizeUserPermissionsDelegate NeutralizeUserPermissions { get; set; }
        public NeutralizeRolePermissionsDelegate NeutralizeRolePermissions { get; set; }
        public SetUserAdministratorDelegate SetUserAdministrator { get; set; }
        public SetRoleAdministratorDelegate SetRoleAdministrator { get; set; }
        public CreateUserPermissionsDelegate CreateUserPermissions { get; set; }
        public CreateRolePermissionsDelegate CreateRolePermissions { get; set; }
        public GetUserPermissionsDelegate GetUserPermissions { get; set; }
        public GetRolePermissionsDelegate GetRolePermissions { get; set; }
        public SetUserPermissionsDelegate SetUserPermissions { get; set; }
        public SetRolePermissionsDelegate SetRolePermissions { get; set; }
        public ApplyMappingsDelegate ApplyMappings { get; set; }
        public DeleteUserPermissionsDelegate DeleteUserPermissions { get; set; }
        public DeleteRolePermissionsDelegate DeleteRolePermissions { get; set; }
#endif
        #endregion

        private readonly PermissionConfiguration config;

        public PermissionEngine(PermissionConfiguration config)
        {
            PermissionSettings.PrecompiledScripts = config.PrecompiledScripts;
            PermissionSettings.UpdateRolePermissions = false;
            PermissionSettings.UpdateUserPermissions = false;
            this.config = config;

            UpdateUserPermissions += VanillaUpdateUserPermissions;
            UpdateRolePermissions += VanillaUpdateRolePermissions;
            GrantUserPermissions += VanillaGrantUserPermissions;
            GrantRolePermissions += VanillaGrantRolePermissions;
            RevokeUserPermissions += VanillaRevokeUserPermissions;
            RevokeRolePermissions += VanillaRevokeRolePermissions;
            NeutralizeUserPermissions += VanillaNeutralizeUserPermissions;
            NeutralizeRolePermissions += VanillaNeutralizeRolePermissions;
            SetUserAdministrator += VanillaSetUserAdministrator;
            SetRoleAdministrator += VanillaSetRoleAdministrator;
            CreateUserPermissions += VanillaCreateUserPermissions;
            CreateRolePermissions += VanillaCreateRolePermissions;
            GetUserPermissions += VanillaGetUserPermissions;
            GetRolePermissions += VanillaGetRolePermissions;
            SetUserPermissions += VanillaSetUserPermissions;
            SetRolePermissions += VanillaSetRolePermissions;
            ApplyMappings += VanillaApplyMappings;
            DeleteUserPermissions += VanillaDeleteUserPermissions;
            DeleteRolePermissions += VanillaDeleteRolePermissions;

            Initialize();
        }

        private void Initialize()
        {

            if(!Directory.Exists("./config/permissions"))
            {
                Console.WriteLine("Missing permission configuration, aborting startup. Please contact the InsanityBot team immediately" +
                    "\nPress any key to continue...");
                Console.ReadKey();
                Process.GetCurrentProcess().Kill();
            }

            // check for default mappings and declarations
            if(!File.Exists("./config/permissions/vanilla.mappings.json"))
            {
                Console.WriteLine("Missing vanilla permission mappings, aborting startup. Please contact the InsanityBot team immediately" +
                    "\nPress any key to continue...");
                Console.ReadKey();
                Process.GetCurrentProcess().Kill();
            }

            if(!File.Exists("./config/permissions/vanilla.pdecl.json"))
            {
                Console.WriteLine("Missing vanilla permission declaration, aborting startup. Please contact the InsanityBot team immediately" +
                    "\nPress any key to continue...");
                Console.ReadKey();
                Process.GetCurrentProcess().Kill();
            }

            // create role directory
            if (!Directory.Exists(DefaultPermissionFileSpecifications.Role.Path))
                Directory.CreateDirectory(DefaultPermissionFileSpecifications.Role.Path);

            // create script directory
            if (!Directory.Exists(DefaultPermissionFileSpecifications.Script.Path))
                Directory.CreateDirectory(DefaultPermissionFileSpecifications.Script.Path);

            // create mapping directory
            if (!Directory.Exists("./mod-data/permissions/mappings"))
                Directory.CreateDirectory("./mod-data/permissions/mappings");

            // create declaration directory
            if (!Directory.Exists("./mod-data/permissions/declarations"))
                Directory.CreateDirectory("./mod-data/permissions/declarations");

            // check whether default permissions are up-to-date
            if (ShouldUpdateDefaultPermissions())
                UpdateDefaultPermissions();

            // check whether mappings are up-to-date
            if (ShouldUpdateMappings())
                UpdateMappings();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Boolean ShouldUpdateDefaultPermissions()
        {
            if(!File.Exists("./config/permissions/default.json"))
            {
                File.Create("./config/permissions/default.json").Close();
                return true;
            }

            if(!File.Exists("./cache/permissions/intermediate/default"))
            {
                File.Create("./cache/permissions/intermediate/default").Close();
                return true;
            }

            StreamReader reader = new("./cache/permissions/intermediate/default");
            String[] checksums = JsonConvert.DeserializeObject<String[]>(reader.ReadToEnd());

            var modFiles = from x in Directory.GetFiles("./mod-data/permissions/declarations")
                           where x.EndsWith(".pdecl.json")
                           select x;

            foreach (var v in modFiles)
                if (!checksums.Contains(v.GetSHA512Checksum()))
                    return true;

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void UpdateDefaultPermissions()
        {
            DefaultPermissions permissions = DefaultPermissions.Empty;

            StreamReader reader = new("./config/permissions/vanilla.pdecl.json");
            permissions += JsonConvert.DeserializeObject<PermissionDeclaration[]>(reader.ReadToEnd());
            reader.Close();

            var modFiles = from x in Directory.GetFiles("./mod-data/permissions/declarations")
                           where x.EndsWith(".pdecl.json")
                           select x;

            foreach(var v in modFiles)
            {
                reader = new(v);
                permissions += JsonConvert.DeserializeObject<PermissionDeclaration[]>(reader.ReadToEnd());
                reader.Close();
            }

            if (!File.Exists("./config/permissions/default.json"))
                File.Create("./config/permissions/default.json").Close();

            StreamWriter writer = new("./config/permissions/default.json");
            writer.Write(JsonConvert.SerializeObject(permissions));
            writer.Close();

            List<String> checksums = new();
            checksums.Add("./config/permissions/vanilla.pdecl.json".GetSHA512Checksum());

            foreach (var v in modFiles)
                checksums.Add(v.GetSHA512Checksum());

            writer = new("./cache/permissions/intermediate/default");
            writer.Write(JsonConvert.SerializeObject(checksums));
            writer.Close();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Boolean ShouldUpdateMappings()
        {
            if(!File.Exists("./config/permissions/mappings.json"))
            {
                File.Create("./config/permissions/mappings.json").Close();
                return true;
            }

            if(!File.Exists("./cache/permissions/intermediate/mappings"))
            {
                File.Create("./cache/permissions/intermediate/mappings").Close();
                return true;
            }

            StreamReader reader = new("./cache/permissions/intermediate/mappings");
            String[] checksums = JsonConvert.DeserializeObject<String[]>(reader.ReadToEnd());

            var modFiles = from x in Directory.GetFiles("./mod-data/permissions/mappings")
                           where x.EndsWith(".mappings.json")
                           select x;

            foreach (var v in modFiles)
                if (!checksums.Contains(v.GetSHA512Checksum()))
                    return true;

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void UpdateMappings()
        {
            StreamReader reader = new("./config/permissions/vanilla.mappings.json");
            PermissionMapping mappings = JsonConvert.DeserializeObject<PermissionMapping>(reader.ReadToEnd());
            reader.Close();

            var modFiles = from x in Directory.GetFiles("./mod-data/permissions/mappings")
                           where x.EndsWith(".mappings.json")
                           select x;

            foreach(var v in modFiles)
            {
                reader = new(v);
                mappings += JsonConvert.DeserializeObject<PermissionMapping>(reader.ReadToEnd());
                reader.Close();
            }

            StreamWriter writer = new($"./config/permissions/mappings.json");
            writer.Write(JsonConvert.SerializeObject(mappings));
            writer.Close();

            List<String> checksums = new();
            checksums.Add("./config/permissions/vanilla.mappings.json".GetSHA512Checksum());

            foreach (var v in modFiles)
                checksums.Add(v.GetSHA512Checksum());

            writer = new("./cache/permissions/intermediate/mappings");
            writer.Write(JsonConvert.SerializeObject(checksums));
            writer.Close();
        }

        private void VanillaDeleteRolePermissions(UInt64 UserId)
        {
            throw new NotImplementedException();
        }

        private void VanillaDeleteUserPermissions(UInt64 UserId)
        {
            throw new NotImplementedException();
        }

        private RolePermissions VanillaApplyMappings(DiscordRole Role)
        {
            throw new NotImplementedException();
        }

        private void VanillaSetRolePermissions(RolePermissions Role)
        {
            throw new NotImplementedException();
        }

        private void VanillaSetUserPermissions(UserPermissions User)
        {
            throw new NotImplementedException();
        }

        private RolePermissions VanillaGetRolePermissions(UInt64 RoleId)
        {
            throw new NotImplementedException();
        }

        private UserPermissions VanillaGetUserPermissions(UInt64 UserId)
        {
            throw new NotImplementedException();
        }

        private void VanillaCreateRolePermissions(UInt64 RoleId)
        {
            throw new NotImplementedException();
        }

        private void VanillaCreateUserPermissions(UInt64 UserId)
        {
            throw new NotImplementedException();
        }

        private void VanillaSetRoleAdministrator(UInt64 RoleId, String[] Permissions)
        {
            throw new NotImplementedException();
        }

        private void VanillaSetUserAdministrator(UInt64 UserId, String[] Permissions)
        {
            throw new NotImplementedException();
        }

        private void VanillaNeutralizeRolePermissions(UInt64 RoleId, String[] Permissions)
        {
            throw new NotImplementedException();
        }

        private void VanillaNeutralizeUserPermissions(UInt64 UserId, String[] Permissions)
        {
            throw new NotImplementedException();
        }

        private void VanillaRevokeRolePermissions(UInt64 RoleId, String[] Permissions)
        {
            throw new NotImplementedException();
        }

        private void VanillaRevokeUserPermissions(UInt64 UserId, String[] Permissions)
        {
            throw new NotImplementedException();
        }

        private void VanillaGrantRolePermissions(UInt64 RoleId, String[] Permissions)
        {
            throw new NotImplementedException();
        }

        private void VanillaGrantUserPermissions(UInt64 UserId, String[] Permissions)
        {
            throw new NotImplementedException();
        }

        private void VanillaUpdateRolePermissions(UInt64 RoleId)
        {
            throw new NotImplementedException();
        }

        private void VanillaUpdateUserPermissions(UInt64 UserId)
        {
            throw new NotImplementedException();
        }
    }
}
