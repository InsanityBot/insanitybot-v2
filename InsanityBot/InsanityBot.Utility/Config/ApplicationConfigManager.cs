using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

using InsanityBot.Utility.Config.Exceptions;
using InsanityBot.Utility.Config.Reference;

namespace InsanityBot.Utility.Config
{
    /// <summary>
    /// Main management class for applications
    /// </summary>
    public static class ApplicationConfigManager
    {
        public static List<String> Applications { get; set; }


        private static void Serialize(ApplicationConfigEntry entry)
        {
            // validate file existence
            if (!Directory.Exists("./config/apps"))
                Directory.CreateDirectory("./config/apps");

            if (!Exists(entry.Identifier.ToLower()))
                File.Create($"./config/apps/{entry.Identifier.ToLower()}.xml");

            // serialize full data
            FileStream writer = new FileStream($"./config/apps/{entry.Identifier.ToLower()}.xml", FileMode.Truncate);
            XmlSerializer serializer = new XmlSerializer(typeof(ApplicationConfigEntry));
            serializer.Serialize(writer, entry);
            writer.Close();
        }

        private static ApplicationConfigEntry Deserialize(String Identifier)
        {
            if (!Exists(Identifier.ToLower()))
                throw new ArgumentException($"No config with the identifier {Identifier.ToLower()} could be found.");

            StreamReader reader = new StreamReader($"./config/apps/{Identifier.ToLower()}.xml");
            XmlSerializer deserializer = new XmlSerializer(typeof(ApplicationConfigEntry));
            ApplicationConfigEntry returnValue = (ApplicationConfigEntry)deserializer.Deserialize(reader);

            reader.Close();
            return returnValue;
        }

        private static void RegisterNewApplication(String Identifier)
        {
            // update the list
            FileStream updater = new FileStream($"./config/apps/applications.xml", FileMode.Open);
            XmlSerializer serializer = new XmlSerializer(typeof(List<String>));

            Applications = ((List<String>)serializer.Deserialize(updater))
                .Append(Identifier.ToLower())
                .ToList(); //update the list

            updater.SetLength(0);
            updater.Flush(); // clear the file to avoid corruption

            serializer.Serialize(updater, Applications);
        }

        /// <summary>
        /// Returns whether the specified application identifier has a corresponding file
        /// </summary>
        public static Boolean Exists(String Identifier)
            => File.Exists($"./config/apps/{Identifier}.xml");

        /// <summary>
        /// Should be called when loading the bot. Will cache existing applications to provide faster validity checks.
        /// </summary>
        public static void Initialize()
        {
            StreamReader reader = new StreamReader("./config/apps/applications.xml");
            XmlSerializer deserializer = new XmlSerializer(typeof(List<String>));

            Applications = (List<String>)deserializer.Deserialize(reader);
            reader.Close();
        }

        #region CreateNewApplication
        /// <summary>
        /// Registers and serializes a new entry
        /// </summary>
        public static void CreateNewApplication(ApplicationConfigEntry entry)
        {
            if (Applications.Contains(entry.Identifier.ToLower()))
                throw new ApplicationOverrideException
                {
                    File = new Uri($"./config/apps/{entry.Identifier.ToLower()}.xml")
                };

            RegisterNewApplication(entry.Identifier);
            Serialize(entry);
        }

        /// <summary>
        /// Registers and serializes a new entry
        /// </summary>
        /// <param name="Identifier">Application ID. Needs to be a single word</param>
        /// <param name="Enabled">Whether the application is executable by members or not</param>
        /// <param name="RequiredTime">Time members need to be in the server to apply</param>
        /// <param name="RequiredRole">Role members need to have before applying</param>
        /// <param name="Questions">Questions that will be asked by the bot automatically when the application is parsed. Caps out at 10 questions.</param>
        public static void CreateNewApplication(String Identifier,
                                                 Boolean Enabled = true,
                                                 Nullable<DateTimeOffset> RequiredTime = null,
                                                 Nullable<UInt64> RequiredRole = null,
                                                 params String[] Questions)
        {
            ApplicationConfigEntry entry = new ApplicationConfigEntry
            {
                Identifier = Identifier,
                Enabled = Enabled,
                RequiredTime = RequiredTime,
                RequiredDiscordRoles = new List<Nullable<UInt64>>().Append(RequiredRole)
                                                                   .ToList(),
                Questions = Questions.ToList()
            };
            CreateNewApplication(entry);
        }

        /// <summary>
        /// Registers and serializes a new entry
        /// </summary>
        /// <param name="Identifier">Application ID. Needs to be a single word</param>
        /// <param name="Enabled">Whether the application is executable by members or not</param>
        /// <param name="RequiredTime">Time members need to be in the server to apply</param>
        /// <param name="Question">A singular question that will be asked by the bot automatically when the application is parsed.</param>
        /// <param name="RequiredRoles">Up to ten role IDs members need to have before applying</param>
        public static void CreateNewApplication(String Identifier,
                                                Boolean Enabled = true,
                                                Nullable<DateTimeOffset> RequiredTime = null,
                                                String Question = null,
                                                params Nullable<UInt64>[] RequiredRoles)
        {
            ApplicationConfigEntry entry = new ApplicationConfigEntry
            {
                Identifier = Identifier,
                Enabled = Enabled,
                RequiredTime = RequiredTime,
                Questions = new List<String>().Append(Question)
                                              .ToList(),
                RequiredDiscordRoles = RequiredRoles.ToList()
            };
            CreateNewApplication(entry);
        }

        /// <summary>
        /// Registers and serializes a new entry
        /// </summary>
        /// <param name="Identifier">Application ID. Needs to be a single word</param>
        /// <param name="Enabled">Whether the application is executable by members or not</param>
        /// <param name="RequiredTime">Time members need to be in the server to apply</param>
        /// <param name="Questions">Questions that will be asked by the bot automatically when the application is parsed. Caps out at 10 questions.</param>
        /// <param name="RequiredRoles">Up to ten role IDs members need to have before applying</param>
        public static void CreateNewApplication(String Identifier,
                                                Boolean Enabled = true,
                                                Nullable<DateTimeOffset> RequiredTime = null,
                                                String[] Questions = null,
                                                Nullable<UInt64>[] RequiredRoles = null)
        {
            ApplicationConfigEntry entry = new ApplicationConfigEntry
            {
                Identifier = Identifier,
                Enabled = Enabled,
                RequiredTime = RequiredTime,
                RequiredDiscordRoles = RequiredRoles.ToList(),
                Questions = Questions.ToList()
            };
            CreateNewApplication(entry);
        }
        #endregion


        // dont use these for registering new entries, the registration performs additional validity checks and caches it

        /// <summary>
        /// Deserializes an application config entry
        /// </summary>
        public static ApplicationConfigEntry GetApplication(String Identifier)
            => Deserialize(Identifier);

        /// <summary>
        /// Serializes an application config entry
        /// </summary>  
        public static void SetApplication(ApplicationConfigEntry entry)
            => Serialize(entry);
    }
}
