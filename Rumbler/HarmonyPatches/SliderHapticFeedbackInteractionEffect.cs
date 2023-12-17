using HarmonyLib;
using IPA.Utilities;
using Libraries.HM.HMLib.VR;

namespace Rumbler.HarmonyPatches
{
    [HarmonyPatch(typeof(SliderHapticFeedbackInteractionEffect), "Start")]
    internal class SliderHapticFeedbackInteractionEffect_Start
    {
        private static void Postfix(SliderHapticFeedbackInteractionEffect __instance)
        {
            var rumblePreset = __instance.GetField<HapticPresetSO, SliderHapticFeedbackInteractionEffect>("_hapticPreset");
            if (rumblePreset != null)
            {
                rumblePreset._strength = 0f;
            }
        }
    }
}