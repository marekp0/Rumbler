using HarmonyLib;
using Libraries.HM.HMLib.VR;
using UnityEngine.XR;

namespace Rumbler.HarmonyPatches
{
    [HarmonyPatch(typeof(UnityXRHelper), "TriggerHapticPulse")]
    internal class UnityXRHelper_TriggerHapticPulse
    {
        private static bool Prefix(HapticFeedbackManager __instance, XRNode node, float duration, float strength, float frequency)
        {
            Plugin.Log?.Info($"handler={RumblerController.Instance.GetHapticsHandler(node)}");

            RumblerController.Instance.GetHapticsHandler(node)?.TriggerHapticPulse(strength, duration, frequency);
            return false;
        }
    }

    [HarmonyPatch(typeof(UnityXRHelper), "StopHaptics")]
    internal class UnityXRHelper_StopHaptics
    {
        private static bool Prefix(HapticFeedbackManager __instance, XRNode node)
        {
            RumblerController.Instance.GetHapticsHandler(node)?.StopHaptics();
            return false;
        }
    }
}