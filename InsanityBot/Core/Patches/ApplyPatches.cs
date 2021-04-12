using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HarmonyLib;

namespace InsanityBot.Core.Patches
{
    public static class ApplyPatches
    {
        public static void ApplyHarmonyPatches()
        {
            Harmony harmony = new("net.insanitybot.patch");
            harmony.PatchAll();
        }
    }
}
