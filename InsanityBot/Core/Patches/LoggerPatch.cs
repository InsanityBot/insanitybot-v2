using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using DSharpPlus;

using HarmonyLib;

using Microsoft.Extensions.Logging;

namespace InsanityBot.Core.Patches
{
    [HarmonyPatch]
    internal class LoggerPatch // Harmony patch to fix console spam by ratelimit errors
    {
        public static MethodBase TargetMethod()
        {
            return typeof(DefaultLogger).GetMethod("Log").MakeGenericMethod(typeof(String));
        }

        public static Boolean Prefix(EventId eventId)
        {
            if (eventId.Name == "RatelimitPre")
                return false;
            if (eventId.Name == "RatelimitHit")
                return false;

            return true;
        }
    }
}
