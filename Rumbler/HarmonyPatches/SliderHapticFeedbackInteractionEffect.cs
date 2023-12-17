using HarmonyLib;
using IPA.Utilities;
using Libraries.HM.HMLib.VR;
using Rumbler.Configuration;

namespace Rumbler.HarmonyPatches
{
    [HarmonyPatch(typeof(SliderHapticFeedbackInteractionEffect), "Start")]
    internal class SliderHapticFeedbackInteractionEffect_Start
    {
        private static void Postfix(SliderHapticFeedbackInteractionEffect __instance)
        {
            var hapticPreset = __instance.GetField<HapticPresetSO, SliderHapticFeedbackInteractionEffect>("_hapticPreset");
            PluginConfig.Instance.Slider.CopyTo(hapticPreset);
        }
    }
}