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

    [HarmonyPatch(typeof(Missile), "Detonate")]
    public static class Missile_Detonate_Patch
    {
        private static readonly AccessTools.FieldRef<Missile, float> HitpointsRef =
            AccessTools.FieldRefAccess<Missile, float>("hitpoints");

        private static readonly AccessTools.FieldRef<Missile, object> WarheadRef =
            AccessTools.FieldRefAccess<Missile, object>("warhead");

        private static readonly Type WarheadType = AccessTools.Inner(typeof(Missile), "Warhead");

        private static readonly AccessTools.FieldRef<object, bool> ArmedRef =
            AccessTools.FieldRefAccess<bool>(WarheadType, "Armed");

        public static void Prefix(Missile __instance, bool hitArmor, bool hitTerrain)
        {
            if (__instance == null) return;
            
            float hitpoints = HitpointsRef(__instance);
            
            if (hitpoints <= 0f || hitArmor || hitTerrain)
            {
                object warhead = WarheadRef(__instance);
                if (warhead != null)
                {
                    // Zero-allocation field ref updates
                    ArmedRef(warhead) = true;
                }
            }
        }
    }

}