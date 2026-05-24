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

        private void Awake()
        {
            Instance = this;
            var harmony = new Harmony("com.kiss");
            harmony.PatchAll();
            Log("KISS Mod loaded, Muach!");
        }

        public void Log(string msg)
        {
            Logger.LogInfo(msg);
        }
    }

    [HarmonyPatch]
    public static class Warhead_Detonate_Patch
    {
        public static System.Reflection.MethodBase TargetMethod()
        {
            return AccessTools.Method(AccessTools.Inner(typeof(Missile), "Warhead"), "Detonate");
        }

        public static void Prefix(ref bool armed)
        {
            armed = true;
        }
    }

    [HarmonyPatch(typeof(Missile), "Awake")]
    public static class Missile_Awake_Patch
    {
        private static readonly AccessTools.FieldRef<Missile, float> GLimitRef =
            AccessTools.FieldRefAccess<Missile, float>("gLimit");

        public static void Postfix(Missile __instance)
        {
            if (__instance == null) return;
            GLimitRef(__instance) = float.MaxValue;
        }
    }
}
