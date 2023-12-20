using HarmonyLib;
using Libraries.HM.HMLib.VR;
using IPA.Utilities;
using Rumbler.Configuration;
using System.Reflection;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;
using UnityEngine.XR;

namespace Rumbler.HarmonyPatches
{
    [HarmonyPatch(typeof(ObstacleSaberSparkleEffectManager), "Update")]
    internal class ObstacleSaberSparkleEffectManager_Update
    {
        static MethodInfo origMethod = AccessTools.Method(typeof(HapticFeedbackManager), "PlayHapticFeedback");
        static MethodInfo newMethod = AccessTools.Method(typeof(ObstacleSaberSparkleEffectManager_Update), "PlayHapticFeedback");

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
                Plugin.Log?.Error("Failed to find call to HapticFeedbackManager.PlayHapticFeedback() in ObstacleSaberSparkleEffectManager.Update()");
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