using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HarmonyLib;
using IPA.Utilities;
using Libraries.HM.HMLib.VR;


namespace Rumbler.HarmonyPatches
{
    [HarmonyPatch(typeof(NoteCutHapticEffect), "HitNote")]
    internal class NoteCutHapticEffect_HitNote
    {
        private static void Prefix(NoteCutHapticEffect __instance)
        {
            Plugin.Log?.Info("here");
            var rumblePreset = __instance.GetField<HapticPresetSO, NoteCutHapticEffect>("_normalPreset");
            if (rumblePreset != null)
            {
                rumblePreset._strength = 0f;
            }
        }
    }
}
