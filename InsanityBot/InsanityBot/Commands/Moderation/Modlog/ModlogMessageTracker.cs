using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsanityBot.Commands.Moderation.Modlog
{
    internal class ModlogMessageTracker
    {
        internal struct MessageTrackerEntry
        {
            public UInt64 MessageId { get; set; }
            public UInt64 UserId { get; set; }
            public Byte Page { get; set; }
            public LogType Type { get; set; }
        }

        public enum LogType
        {
            Modlog,
            VerbalLog
        }

        private static List<MessageTrackerEntry> messageTracker;

        public static List<MessageTrackerEntry> MessageTracker { get => messageTracker; }
        public static void AddTrackedMessage(MessageTrackerEntry entry)
        {
            if (messageTracker.Count >= 20)
                messageTracker.RemoveAt(0);

            if(IsTracked(entry.MessageId))
            {
                var messages = from v in messageTracker
                               where v.MessageId == entry.MessageId
                               select v;

                foreach (var v in messages)
                    messageTracker.Remove(v);
            }

            messageTracker.Add(entry);
        }

        public static Boolean IsTracked(UInt64 messageId)
        {
            var messages = from v in messageTracker
                           where v.MessageId == messageId
                           select v;
            if (messages.ToList().Count != 0)
                return true;
            return false;
        }

        public static void CreateTracker()
            => messageTracker = new List<MessageTrackerEntry>();
    }
}
