using HarmonyLib;
using IPA.Utilities;
using Libraries.HM.HMLib.VR;
using Rumbler.Configuration;

namespace Rumbler.HarmonyPatches
{
    [HarmonyPatch(typeof(SaberClashEffect), "Start")]
    internal class SaberClashEffect_Start
    {
        private static void Postfix(SaberClashEffect __instance)
        {
            var rumblePreset = __instance.GetField<HapticPresetSO, SaberClashEffect>("_rumblePreset");
            PluginConfig.Instance.SaberClash.CopyTo(rumblePreset);
        }
    }
}