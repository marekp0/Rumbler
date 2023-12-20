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
        private static void Prefix(NoteCutHapticEffect __instance, SaberType saberType, NoteCutHapticEffect.Type type)
        {
            // figure out which rumble params to use
            var rumbleParams = type switch
            {
                NoteCutHapticEffect.Type.Normal => PluginConfig.Instance.NoteCutNormal,
                NoteCutHapticEffect.Type.ShortNormal => PluginConfig.Instance.NoteCutShortNormal,
                NoteCutHapticEffect.Type.ShortWeak => PluginConfig.Instance.NoteCutShortWeak,
                NoteCutHapticEffect.Type.Bomb => PluginConfig.Instance.NoteCutBomb,
                NoteCutHapticEffect.Type.BadCut => PluginConfig.Instance.NoteCutBadCut,
                _ => PluginConfig.Instance.NoteCutNormal
            };

            // play the rumble
            var node = SaberTypeExtensions.Node(saberType);
            var player = RumblerController.Instance?.HapticFeedbackPlayer;
            player?.PlayHapticFeedback(node, rumbleParams.ToRumbleInfo());
        }
    }
}
