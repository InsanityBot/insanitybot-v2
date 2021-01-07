using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace InsanityBot.Utility.Timers
{
    public static class TimeHandler
    {
        public static void Start()
        {
            Countdown = new System.Timers.Timer
            {
                Interval = 5000
            };
            Countdown.Elapsed += CountdownElapsed;

            Countdown.Start();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private static void CountdownElapsed(Object sender, System.Timers.ElapsedEventArgs e)
        {
            if (!Directory.Exists("./data/timers"))
            {
                Directory.CreateDirectory("./data/timers");
                return;
            }

            //knowing that it exists, proceed to read contents

            if (Directory.GetFiles("./data/timers").Length == 0)
                return;

            //ok, it exists and has file contents. time to read.

            List<Timer> ActiveTimers = new List<Timer>();

            StreamReader reader = null;

            foreach (String s in Directory.GetFiles("./data/timers"))
            {
                //keep this from throwing a fatal error
                //if an exception occurs, it just means the timer adding procedure took a little longer than usual
                try
                {
                    reader = new StreamReader(File.OpenRead(s));
                    ActiveTimers.Add(JsonConvert.DeserializeObject<Timer>(reader.ReadToEnd()));
                    reader.Close();
                }
                catch { }
            }

            foreach (Timer t in ActiveTimers)
            {
                if (t == null)
                    continue;
                if (!t.CheckExpiry())
                    continue;
                else
                    return;
            }

            Countdown.Start();
        }

        public static void AddTimer(Timer timer)
        {
            Countdown.Stop();

            if (!Directory.Exists("./data/timers"))
                Directory.CreateDirectory("./data/timers");

            StreamWriter writer;

            if (!File.Exists($"./data/timers/{timer.Identifier}"))
                File.Create($"./data/timers/{timer.Identifier}").Close();
            writer = new StreamWriter(File.Open($"./data/timers/{timer.Identifier}", FileMode.Truncate));

            writer.Write(JsonConvert.SerializeObject(timer));

            writer.Close();

            Thread.Sleep(500);
            Countdown.Start();
        }

        public static void ReenableTimer()
        {
            Thread.Sleep(250);

            Countdown.Start();
        }

        public static void DisableTimer()
        {
            Countdown.Stop();
        }

        private static System.Timers.Timer Countdown { get; set; }
    }
}
