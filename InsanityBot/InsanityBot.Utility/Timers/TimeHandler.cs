using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;

using Newtonsoft.Json;

namespace InsanityBot.Utility.Timers
{
    public static class TimeHandler
    {
        public static void Start()
        {
            if(!Directory.Exists("./data/timers"))
            {
                Directory.CreateDirectory("./data/timers");
                return;
            }

            //knowing that it exists, proceed to read contents

            if (Directory.GetFiles("./data/timers").Length == 0)
                return;

            //ok, it exists and has file contents. time to read.

            StreamReader reader;

            foreach (String s in Directory.GetFiles("./data/timers"))
            {
                reader = new StreamReader(File.OpenRead(s));
                ActiveTimers.Add(JsonConvert.DeserializeObject<Timer>(reader.ReadToEnd()));
            }
        }

        public static void Stop()
        {
            if (!Directory.Exists("./data/timers"))
                Directory.CreateDirectory("./data/timers");

            if (ActiveTimers.Count == 0)
                return;

            StreamWriter writer;

            foreach(Timer t in ActiveTimers)
            {
                if (File.Exists($"./data/timers/{t.Identifier}"))
                    File.Create($"./data/timers/{t.Identifier}").Close();
                writer = new StreamWriter(File.Open($"./data/timers/{t.Identifier}", FileMode.Truncate));

                writer.Write(JsonConvert.SerializeObject(t));
            }
        }

        public static List<Timer> ActiveTimers { get; set; }
    }
}
