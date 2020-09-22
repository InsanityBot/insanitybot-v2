using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

using InsanityBot.Utility.Config;

namespace InsanityBot
{
    public partial class InsanityBot
    {
        public static async Task HardReset()
        {
            //fully resets all bot configs, data files etc.
            Directory.Delete("./config", true);
            Directory.Delete("./data", true);
            await Initialize();
        }

        public static async Task Initialize()
        {
            //generates all bot configs and data files that dont exist yet
            if (!Directory.Exists("./config"))
                Directory.CreateDirectory("./config");

            if (!Directory.Exists("./data"))
                Directory.CreateDirectory("./data");

            if (!File.Exists("./config/main.json"))
            {
                File.Create("./config/main.json");
                await CreateMainConfig();
            }
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public static async Task CreateMainConfig()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            MainConfigManager.SerializeMainConfiguration();
        }
    }
}
