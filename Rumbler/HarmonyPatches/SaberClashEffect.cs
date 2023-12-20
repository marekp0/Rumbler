using HarmonyLib;
using IPA.Utilities;
using Libraries.HM.HMLib.VR;
using Rumbler.Configuration;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Reflection;
using UnityEngine;
using UnityEngine.XR;

namespace Rumbler.HarmonyPatches
{
    [HarmonyPatch(typeof(SaberClashEffect), "LateUpdate")]
    internal class SaberClashEffect_LateUpdate
    {
        static MethodInfo origMethod = AccessTools.Method(typeof(HapticFeedbackManager), "PlayHapticFeedback");
        static MethodInfo newMethod = AccessTools.Method(typeof(SaberClashEffect_LateUpdate), "PlayHapticFeedback");

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
                Plugin.Log?.Error("Failed to find call to HapticFeedbackManager.PlayHapticFeedback() in SaberClashEffect.Update()");
            }
        }

        private static void PlayHapticFeedback(HapticFeedbackManager instance, XRNode node, HapticPresetSO preset)
        {
            var rumbleInfo = PluginConfig.Instance.SaberClash.ToRumbleInfo();
            rumbleInfo.rumbleDuration = Time.deltaTime;
            var player = RumblerController.Instance?.HapticFeedbackPlayer;
            player?.PlayHapticFeedback(node, rumbleInfo);
        }
    }
}