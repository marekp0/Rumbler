using HarmonyLib;
using Libraries.HM.HMLib.VR;
using IPA.Utilities;
using Rumbler.Configuration;

namespace Rumbler.HarmonyPatches
{
    [HarmonyPatch(typeof(ObstacleSaberSparkleEffectManager), "Start")]
    internal class ObstacleSaberSparkleEffectManager_SpawnEffect
    {
        private static void Postfix(ObstacleSaberSparkleEffectManager __instance)
        {
            var rumblePreset = __instance.GetField<HapticPresetSO, ObstacleSaberSparkleEffectManager>("_rumblePreset");
            PluginConfig.Instance.Obstacle.CopyTo(rumblePreset);
        }
    }
}