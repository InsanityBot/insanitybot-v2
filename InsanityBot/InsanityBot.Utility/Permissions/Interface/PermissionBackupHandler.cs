using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsanityBot.Utility.Permissions.Interface
{
    public static class PermissionBackupHandler
    {
        public static void BackupRoles(Boolean auto = false)
        {
            if(auto && !OperatingSystem.IsLinux())
            {
                Console.WriteLine("\n" +
                    "Automated backups of any kind are only supported on Linux host systems.\n");
                return;
            }

            DirectoryInfo roleDirectory = new("./data/role-permissions");
            DirectoryInfo backupDirectory = Directory.CreateDirectory($"./permission-backups/data/role-permissions");

            foreach (var v in roleDirectory.GetFiles())
                v.CopyTo(backupDirectory.FullName);
        }

        public static void BackupUsers(Boolean auto = false)
        {
            if (auto && !OperatingSystem.IsLinux())
            {
                Console.WriteLine("\n" +
                    "Automated backups of any kind are only supported on Linux host systems.\n");
                return;
            }

            String[] userPaths = Directory.GetDirectories("./data");
            DirectoryInfo backupDirectory = Directory.CreateDirectory($"./permission-backups/data/user-permissions");
            StreamReader reader;
            StreamWriter writer;

            foreach (var v in userPaths)
            {
                FileInfo f = new($"{v}/permissions.json");
                String userId = f.Directory.Name;

                reader = new(f.FullName);
                String fileContent = reader.ReadToEnd();

                writer = new(File.Create($"{backupDirectory.FullName}/{userId}.json"));
                writer.Write(fileContent);

                reader.Close();
                writer.Close();
            }
        }

        public static void RestoreRoles()
        {
            DirectoryInfo roleDirectory = new("./data/role-permissions");
            DirectoryInfo backupDirectory = Directory.CreateDirectory($"./permission-backups/data/role-permissions");

            foreach (var v in backupDirectory.GetFiles())
                v.CopyTo(roleDirectory.FullName);
        }

        public static void RestoreUsers()
        {
            DirectoryInfo userDirectory = Directory.CreateDirectory($"./data");
            String[] backupPaths = Directory.GetFiles($"./permission-backups/data/user-permissions");
            StreamReader reader;
            StreamWriter writer;

            foreach (var v in backupPaths)
            {
                FileInfo file = new(v);
                reader = new(v);
                String fileContent = reader.ReadToEnd();

                writer = new(File.Create($"{userDirectory.FullName}/{file.Name.Split('.')[0]}/permissions.json"));
                writer.Write(fileContent);

                reader.Close();
                writer.Close();
            }
        }

        public static void BackupRole(UInt64 id)
        {

        }

        public static void BackupUser(UInt64 id)
        {

        }

        public static void RestoreRole(UInt64 id)
        {

        }

        public static void RestoreUser(UInt64 id)
        {

        }
    }
}
