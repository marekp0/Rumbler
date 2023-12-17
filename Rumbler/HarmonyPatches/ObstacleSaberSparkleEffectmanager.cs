using HarmonyLib;
using Libraries.HM.HMLib.VR;
using IPA.Utilities;

namespace Rumbler.HarmonyPatches
{
    [HarmonyPatch(typeof(ObstacleSaberSparkleEffectManager), "Start")]
    internal class ObstacleSaberSparkleEffectManager_SpawnEffect
    {
        private static void Postfix(ObstacleSaberSparkleEffectManager __instance)
        {
            var rumblePreset = __instance.GetField<HapticPresetSO, ObstacleSaberSparkleEffectManager>("_rumblePreset");
            if (rumblePreset != null)
            {
                rumblePreset._strength = 0f;
            }
        }
    }
}