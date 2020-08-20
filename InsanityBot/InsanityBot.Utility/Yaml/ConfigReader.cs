using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsanityBot.Utility.Yaml
{
    /// <summary>
    /// Provides static utilities to validate and read the configuration
    /// </summary>
    public static class ConfigReader
    {
        /// <summary>
        /// Version ID the bot is starting with. Used to apply datafixes to update the config.
        /// </summary>
        public static UInt16 DataVersion { get; set; }

        /// <summary>
        /// Gets the value of a specified entry in the YAML file.
        /// </summary>
        /// <param name="FilePath">Path to the configuration file</param>
        /// <param name="YamlPath">Path to the Yaml entry</param>
        /// <returns>The entry value as object.</returns>
        /// <exception cref="NullReferenceException">Thrown if the value does not exist.</exception>"
        /// <exception cref="ArgumentException">Thrown if either the FilePath or the YamlPath are invalild</exception>
        public static async Task<Object> GetConfigValueAsync(String FilePath, String YamlPath)
        {
            StreamReader reader = new StreamReader(FilePath);
            String tmp;

            while (true)
            {
                if ((tmp = await reader.ReadLineAsync()).StartsWith(YamlPath))
                {
                    return tmp.Substring((YamlPath + ": ").Length);
                }
            }
        }

        public static async Task ApplyDatafixesAsync(String FilePath, YamlConfigType config)
        {
            throw new NotImplementedException();
        }
    }
}
