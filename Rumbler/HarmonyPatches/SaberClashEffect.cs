using HarmonyLib;
using IPA.Utilities;
using Libraries.HM.HMLib.VR;

namespace Rumbler.HarmonyPatches
{
    [HarmonyPatch(typeof(SaberClashEffect), "Start")]
    internal class SaberClashEffect_Start
    {
        private static void Postfix(SaberClashEffect __instance)
        {
            var rumblePreset = __instance.GetField<HapticPresetSO, SaberClashEffect>("_rumblePreset");
            if (rumblePreset != null)
            {
                rumblePreset._strength = 0f;
            }
        }
    }
}