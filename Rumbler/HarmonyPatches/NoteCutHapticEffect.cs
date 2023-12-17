using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HarmonyLib;
using IPA.Utilities;
using Libraries.HM.HMLib.VR;
using Rumbler.Configuration;


namespace Rumbler.HarmonyPatches
{
    [HarmonyPatch(typeof(NoteCutHapticEffect), "HitNote")]
    internal class NoteCutHapticEffect_HitNote
    {
        private static void Prefix(NoteCutHapticEffect __instance, NoteCutHapticEffect.Type type)
        {
            var fieldName = type switch
            {
                NoteCutHapticEffect.Type.Normal => "_normalPreset",
                NoteCutHapticEffect.Type.ShortNormal => "_shortNormalPreset",
                NoteCutHapticEffect.Type.ShortWeak => "_shortWeakPreset",
                NoteCutHapticEffect.Type.Bomb => "_bombPreset",
                NoteCutHapticEffect.Type.BadCut => "_badCutPreset",
                _ => "_normalPreset"
            };

            var rumbleParams = type switch
            {
                NoteCutHapticEffect.Type.Normal => PluginConfig.Instance.NoteCutNormal,
                NoteCutHapticEffect.Type.ShortNormal => PluginConfig.Instance.NoteCutShortNormal,
                NoteCutHapticEffect.Type.ShortWeak => PluginConfig.Instance.NoteCutShortWeak,
                NoteCutHapticEffect.Type.Bomb => PluginConfig.Instance.NoteCutBomb,
                NoteCutHapticEffect.Type.BadCut => PluginConfig.Instance.NoteCutBadCut,
                _ => PluginConfig.Instance.NoteCutNormal
            };

            var hapticPreset = __instance.GetField<HapticPresetSO, NoteCutHapticEffect>(fieldName);
            rumbleParams.CopyTo(hapticPreset);
        }
    }
}
