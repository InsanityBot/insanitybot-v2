using System;
using System.Collections.Generic;

using DSharpPlus.Entities;

using InsanityBot.Utility.Permissions.Controller;
using InsanityBot.Utility.Permissions.Model;

namespace InsanityBot.Utility.Permissions.Interface
{
    public static class ModifyPermissions
    {
        #region User
        public static void GrantMemberPermission(this DiscordMember member, params String[] permission)
        {
            UserPermissions permissions = UserPermissionSerializer.Deserialize(member.Id);

            List<String> resolved = permission.ResolveWildcardPermissions();

            foreach(var v in resolved)
            {
                try
                {
                    permissions[v] = true;
                }
                catch(Exception e)
                {
                    Console.WriteLine($"{e}: {e.Message}\n{e.StackTrace}");
                }
            }

            UserPermissionSerializer.Serialize(permissions);
        }

        public static void RevokeMemberPermission(this DiscordMember member, params String[] permission)
        {
            UserPermissions permissions = UserPermissionSerializer.Deserialize(member.Id);

            List<String> resolved = permission.ResolveWildcardPermissions();

            foreach (var v in resolved)
            {
                try
                {
                    permissions[v] = false;
                }
                catch (Exception e)
                {
                    Console.WriteLine($"{e}: {e.Message}\n{e.StackTrace}");
                }
            }

            UserPermissionSerializer.Serialize(permissions);
        }
        #endregion

        #region Role
        public delegate void SyncUserPermissionWithRolePermissionEventHandler(RolePermissions old, RolePermissions updated);

        public static event SyncUserPermissionWithRolePermissionEventHandler SyncUserPermissionWithRolePermissionEvent;

        public static void GrantRolePermission(this DiscordRole member, params String[] permission)
        {
            RolePermissions permissions = RolePermissionSerializer.Deserialize(member.Id), old = permissions;

            List<String> resolved = permission.ResolveWildcardPermissions();

            foreach (var v in resolved)
            {
                try
                {
                    permissions[v] = true;
                }
                catch (Exception e)
                {
                    Console.WriteLine($"{e}: {e.Message}\n{e.StackTrace}");
                }
            }

            SyncUserPermissionWithRolePermissionEvent(old, permissions);
            RolePermissionSerializer.Serialize(permissions);
        }

        public static void RevokeRolePermission(this DiscordRole member, params String[] permission)
        {
            RolePermissions permissions = RolePermissionSerializer.Deserialize(member.Id), old = permissions;

            List<String> resolved = permission.ResolveWildcardPermissions();

            foreach (var v in resolved)
            {
                try
                {
                    permissions[v] = false;
                }
                catch (Exception e)
                {
                    Console.WriteLine($"{e}: {e.Message}\n{e.StackTrace}");
                }
            }

            SyncUserPermissionWithRolePermissionEvent(old, permissions);
            RolePermissionSerializer.Serialize(permissions);
        }
        #endregion

        #region Script
        public static void GrantScriptPermission(this DiscordMember member, params String[] permission)
        {
            ScriptPermissions permissions = ScriptPermissionSerializer.GetScriptPermissions(member.Id);

            List<String> resolved = permission.ResolveWildcardPermissions();

            foreach (var v in resolved)
            {
                try
                {
                    permissions.Permissions[v] = true;
                }
                catch (Exception e)
                {
                    Console.WriteLine($"{e}: {e.Message}\n{e.StackTrace}");
                }
            }

            ScriptPermissionSerializer.SetScriptPermissions(permissions);
        }

        public static void RevokeScriptPermission(this DiscordMember member, params String[] permission)
        {
            ScriptPermissions permissions = ScriptPermissionSerializer.GetScriptPermissions(member.Id);

            List<String> resolved = permission.ResolveWildcardPermissions();

            foreach (var v in resolved)
            {
                try
                {
                    permissions.Permissions[v] = false;
                }
                catch (Exception e)
                {
                    Console.WriteLine($"{e}: {e.Message}\n{e.StackTrace}");
                }
            }

            ScriptPermissionSerializer.SetScriptPermissions(permissions);
        }
        #endregion

        #region Default
        public delegate void SyncUserPermissionWithDefaultPermissionEventHandler(DefaultPermissions old, DefaultPermissions updated);
        public delegate void SyncRolePermissionWithDefaultPermissionEventHandler(DefaultPermissions old, DefaultPermissions updated);

        public static event SyncUserPermissionWithDefaultPermissionEventHandler SyncUserPermissionWithDefaultPermissionEvent;
        public static event SyncRolePermissionWithDefaultPermissionEventHandler SyncRolePermissionWithDefaultPermissionEvent;

        public static void GrantDefaultPermission(this DiscordRole member, params String[] permission)
        {
            DefaultPermissions permissions = DefaultPermissionSerializer.GetDefaultPermissions(), old = permissions;

            List<String> resolved = permission.ResolveWildcardPermissions();

            foreach (var v in resolved)
            {
                try
                {
                    permissions.Permissions[v] = true;
                }
                catch (Exception e)
                {
                    Console.WriteLine($"{e}: {e.Message}\n{e.StackTrace}");
                }
            }

            SyncRolePermissionWithDefaultPermissionEvent(old, permissions);
            SyncUserPermissionWithDefaultPermissionEvent(old, permissions);
            DefaultPermissionSerializer.SetDefaultPermissions(permissions);
        }

        public static void RevokeDefaultPermission(this DiscordRole member, params String[] permission)
        {
            DefaultPermissions permissions = DefaultPermissionSerializer.GetDefaultPermissions(), old = permissions;

            List<String> resolved = permission.ResolveWildcardPermissions();

            foreach (var v in resolved)
            {
                try
                {
                    permissions.Permissions[v] = false;
                }
                catch (Exception e)
                {
                    Console.WriteLine($"{e}: {e.Message}\n{e.StackTrace}");
                }
            }

            SyncRolePermissionWithDefaultPermissionEvent(old, permissions);
            SyncUserPermissionWithDefaultPermissionEvent(old, permissions);
            DefaultPermissionSerializer.SetDefaultPermissions(permissions);
        }
        #endregion
    }
}
