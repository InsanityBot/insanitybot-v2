using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;

namespace InsanityBot.Utility.Timers
{
    public static class TimeHandler
    {
        public static void Start()
        {
            Countdown = new System.Timers.Timer
            {
                Interval = 250
            };
            Countdown.Elapsed += CountdownElapsed;

            if(!Directory.Exists("./cache/timers"))
            {
                Directory.CreateDirectory("./cache/timers");
            }

            //knowing that it exists, proceed to read contents

            Active = new();

            StreamReader reader;

            foreach(String s in Directory.GetFiles("./cache/timers"))
            {
                //keep this from throwing a fatal error
                //if an exception occurs, it just means the timer adding procedure took a little longer than usual
                try
                {
                    reader = new StreamReader(File.OpenRead(s));
                    Active.Add(JsonConvert.DeserializeObject<Timer>(reader.ReadToEnd()));
                    reader.Close();
                }
                catch { }
            }


            Countdown.Start();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private static void CountdownElapsed(Object sender, System.Timers.ElapsedEventArgs e)
        {
            for(int i = 0; i < Active.Count; i++)
            {
                if(Active[i] == null)
                {
                    continue;
                }

                Boolean toRemove = Active[i].CheckExpiry();

                if(toRemove)
                {
                    Active.RemoveAt(i);
                }
            }

            Countdown.Start();
        }

        public static void AddTimer(Timer timer)
        {
            Countdown.Stop();

            StreamWriter writer;

            if(!File.Exists($"./cache/timers/{timer.Identifier}"))
            {
                File.Create($"./cache/timers/{timer.Identifier}").Close();
            }

            writer = new StreamWriter(File.Open($"./cache/timers/{timer.Identifier}", FileMode.Truncate));

            writer.Write(JsonConvert.SerializeObject(timer));

            writer.Close();

            Active.Add(timer);

            Thread.Sleep(50);
            Countdown.Start();
        }

        public static void ReenableTimer()
        {
            Thread.Sleep(250);

            Countdown.Start();
        }

        public static void DisableTimer() => Countdown.Stop();

        private static System.Timers.Timer Countdown { get; set; }
        private static List<Timer> Active { get; set; }
    }
}
