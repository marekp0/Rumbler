using HarmonyLib;
using IPA.Utilities;
using Libraries.HM.HMLib.VR;
using Rumbler.Configuration;
using UnityEngine;

namespace Rumbler.HarmonyPatches
{
    [HarmonyPatch(typeof(SliderHapticFeedbackInteractionEffect), "Vibrate")]
    internal class SliderHapticFeedbackInteractionEffect_Vibrate
    {
        private static bool Prefix(SliderHapticFeedbackInteractionEffect __instance)
        {
            // figure out rumble info
            var rumbleInfo = PluginConfig.Instance.Slider.ToRumbleInfo();
            rumbleInfo.rumbleDuration = Time.deltaTime;

            var saberType = __instance.GetField<SaberType, SliderHapticFeedbackInteractionEffect>("_saberType");
            var player = RumblerController.Instance?.HapticFeedbackPlayer;
            player?.PlayHapticFeedback(saberType.Node(), rumbleInfo);

            return false;
        }
    }
}