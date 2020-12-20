using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

using Newtonsoft.Json;

namespace InsanityBot.Utility.Timers
{
    public static class TimeHandler
    {
        public static void Start()
        {
            ActiveTimers = new List<Timer>();

             if (!Directory.Exists("./data/timers"))
            {
                Directory.CreateDirectory("./data/timers");
                return;
            }

            //knowing that it exists, proceed to read contents

            if (Directory.GetFiles("./data/timers").Length == 0)
                return;

            //ok, it exists and has file contents. time to read.

            StreamReader reader = null;

            foreach (String s in Directory.GetFiles("./data/timers"))
            {
                reader = new StreamReader(File.OpenRead(s));
                ActiveTimers.Add(JsonConvert.DeserializeObject<Timer>(reader.ReadToEnd()));
            }

            reader.Close();

            foreach (Timer t in ActiveTimers)
                //disable the warning, why tf would it exist; this isnt an async method
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                t.CheckExpiry();

            Countdown = new System.Timers.Timer
            {
                AutoReset = true,
                Interval = 500
            };
            Countdown.Elapsed += CountdownElapsed;
        }

        private static void CountdownElapsed(Object sender, System.Timers.ElapsedEventArgs e)
        {
            foreach (Timer t in ActiveTimers)
                t.CheckExpiry();
            //and here we restore the warning again
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        }

        public static void AddTimer(Timer timer)
        {
            ActiveTimers.Add(timer);

            if (!Directory.Exists("./data/timers"))
                Directory.CreateDirectory("./data/timers");

            StreamWriter writer;
            
            if (!File.Exists($"./data/timers/{timer.Identifier}"))
                File.Create($"./data/timers/{timer.Identifier}").Close();
            writer = new StreamWriter(File.Open($"./data/timers/{timer.Identifier}", FileMode.Truncate));

            writer.Write(JsonConvert.SerializeObject(timer));

            writer.Close();           
        }

        private static List<Timer> ActiveTimers{ get; set; }
        private static System.Timers.Timer Countdown { get; set; }
    }
}
