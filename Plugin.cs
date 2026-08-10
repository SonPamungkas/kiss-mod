using System;
using BepInEx;
using HarmonyLib;
using UnityEngine;
namespace KISSMod
{
    [BepInPlugin("neutral.kiss", "KISS Mod (Kinetic Interception Secondary System)", "1.1.1")]
    public class KISSPlugin : BaseUnityPlugin
    {
        public static KISSPlugin Instance;
        private Harmony _harmony;
        private void Awake()
        {
            Instance = this;
            _harmony = new Harmony("neutral.kiss");
            var prefixMethod = new HarmonyMethod(AccessTools.Method(typeof(Warhead_Detonate_Patch), "Prefix"));
            var originalDetonate = AccessTools.Method(typeof(Missile.Warhead), "Detonate");
            if (originalDetonate != null)
            {
                _harmony.Patch(originalDetonate, prefix: prefixMethod);
            }
            Log("KISS Mod loaded, Muach!");
        }
        private void Start()
        {
            var nokwDetonate = AccessTools.Method("NOKW.Patches.KillsLogging.MissileExtensions:Detonate");
            if (nokwDetonate != null)
            {
                var prefixMethod = new HarmonyMethod(AccessTools.Method(typeof(Warhead_Detonate_Patch), "Prefix"));
                _harmony.Patch(nokwDetonate, prefix: prefixMethod);
                Log("KISS Mod: NOKillWeapons compatibility enabled.");
            }
        }
        public void Log(string msg)
        {
            Logger.LogInfo(msg);
        }
    }
    public static class Warhead_Detonate_Patch
    {
        public static void Prefix(ref bool armed)
        {
            armed = true;
        }
    }
}
