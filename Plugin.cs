using System;
using BepInEx;
using HarmonyLib;
using UnityEngine;

namespace KISSMod
{
    [BepInPlugin("com.kiss", "KISS Mod (Kinetic Interception Secondary System)", "1.0.0")]
    public class KISSPlugin : BaseUnityPlugin
    {
        public static KISSPlugin Instance;
        private Harmony _harmony;

        private void Awake()
        {
            Instance = this;
            _harmony = new Harmony("com.kiss");
            
            // Patch standard Awake
            _harmony.Patch(
                AccessTools.Method(typeof(Missile), "Awake"),
                postfix: new HarmonyMethod(AccessTools.Method(typeof(Missile_Awake_Patch), "Postfix"))
            );
            
            var prefixMethod = new HarmonyMethod(AccessTools.Method(typeof(Warhead_Detonate_Patch), "Prefix"));

            // Patch vanilla Warhead.Detonate
            var originalDetonate = AccessTools.Method(AccessTools.Inner(typeof(Missile), "Warhead"), "Detonate");
            if (originalDetonate != null)
            {
                _harmony.Patch(originalDetonate, prefix: prefixMethod);
            }

            Log("KISS Mod loaded, Muach!");
        }

        private void Start()
        {
            // Compatibility: Patch NOKillWeapons' Detonate override if it exists
            // We do this in Start() to avoid load order dependency issues with other mods (like QoL).
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

    public static class Missile_Awake_Patch
    {
        private static readonly AccessTools.FieldRef<Missile, float> GLimitRef =
            AccessTools.FieldRefAccess<Missile, float>("gLimit");

        public static void Postfix(Missile __instance)
        {
            if (__instance == null) return;
            // Disable G-limit so munitions don't die prematurely from physics spikes
            // when hitting the ground at supersonic speeds.
            GLimitRef(__instance) = float.MaxValue;
        }
    }
}