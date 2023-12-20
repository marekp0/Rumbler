using System.Collections.Generic;
using System.Reflection.Emit;
using System.Linq;
using HarmonyLib;
using IPA.Utilities;
using Libraries.HM.HMLib.VR;
using Rumbler.Configuration;
using VRUIControls;
using UnityEngine.XR;
using System.Reflection;

namespace Rumbler.HarmonyPatches
{
    [HarmonyPatch(typeof(VRInputModule), "HandlePointerExitAndEnter")]
    internal class VRInputModule_HandlePointerExitAndEnter
    {
        static MethodInfo origMethod = AccessTools.Method(typeof(HapticFeedbackManager), "PlayHapticFeedback");
        static MethodInfo newMethod = AccessTools.Method(typeof(VRInputModule_HandlePointerExitAndEnter), "PlayHapticFeedback");

        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var found = false;
            foreach (var instruction in instructions)
            {
                // replace call to HapticFeedbackManager.PlayHapticFeedback() with call to our own method
                if (instruction.opcode == OpCodes.Callvirt && (instruction.operand as MethodInfo) == origMethod)
                {
                    yield return new CodeInstruction(OpCodes.Call, newMethod);
                    found = true;
                }
                else
                {
                    yield return instruction;
                }
            }

            if (!found)
            {
                Plugin.Log?.Error("Failed to find call to HapticFeedbackManager.PlayHapticFeedback() in VRInputModule.HandlePointerExitAndEnter()");
            }
        }


        private static void PlayHapticFeedback(HapticFeedbackManager instance, XRNode node, HapticPresetSO preset)
        {
            var rumbleInfo = PluginConfig.Instance.UI.ToRumbleInfo();
            var player = RumblerController.Instance?.HapticFeedbackPlayer;
            player?.PlayHapticFeedback(node, rumbleInfo);
        }
    }
}