using HarmonyLib;
using IPA.Utilities;
using Libraries.HM.HMLib.VR;
using Rumbler.Configuration;
using VRUIControls;

namespace Rumbler.HarmonyPatches
{
    [HarmonyPatch(typeof(VRInputModule), "HandlePointerExitAndEnter")]
    internal class VRInputModule_HandlePointerExitAndEnter
    {
        private static void Prefix(VRInputModule __instance)
        {
            var rumblePreset = __instance.GetField<HapticPresetSO, VRInputModule>("_rumblePreset");
            //PluginConfig.Instance.UI.CopyTo(rumblePreset);
        }
    }
}