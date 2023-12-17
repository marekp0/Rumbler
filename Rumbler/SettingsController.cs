using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.ViewControllers;
using IPA.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine.XR.OpenXR.Input;
using UnityEngine.InputSystem;


namespace Rumbler
{
    [HotReload(RelativePathToLayout = @"Views/Settings.bsml")]
    [ViewDefinition("Rumbler.Views.Settings.bsml")]
    internal class SettingsController : BSMLAutomaticViewController
    {
        private Configuration.PluginConfig conf;
        private Configuration.PluginConfig localConf;

        private const float kContinuousRumbleTestDuration = 2f;


        public SettingsController(Configuration.PluginConfig conf)
        {
            this.conf = conf;
            localConf = new Configuration.PluginConfig();
            localConf.CopyFrom(conf);
        }

        [UIValue("enabled")]
        public bool IsEnabled {
            get { return localConf.IsEnabled; }
            set { localConf.IsEnabled = value; }
        }

        [UIValue("note_strength")]
        public float NoteStrength
        {
            get { return localConf.NoteStrength; }
            set { localConf.NoteStrength = value; }
        }

        [UIValue("note_frequency")]
        public float NoteFrequency
        {
            get { return localConf.NoteFrequency; }
            set { localConf.NoteFrequency = value; }
        }

        [UIValue("note_duration")]
        public float NoteDuration
        {
            get { return localConf.NoteDuration; }
            set { localConf.NoteDuration = value; }
        }

        [UIValue("wall_strength")]
        public float WallStrength
        { 
            get { return localConf.WallStrength;}
            set { localConf.WallStrength = value; }
        }

        [UIValue("wall_frequency")]
        public float WallFrequency
        {
            get { return localConf.WallFrequency; }
            set { localConf.WallFrequency = value; }
        }

        [UIValue("arc_strength")]
        public float ArcStrength
        {
            get { return localConf.ArcStrength; }
            set { localConf.ArcStrength = value; }
        }

        [UIValue("arc_frequency")]
        public float ArcFrequency
        {
            get { return localConf.ArcFrequency; }
            set { localConf.ArcFrequency = value; }
        }

        [UIValue("saber_clash_strength")]
        public float SaberClashStrength
        {
            get { return localConf.SaberClashStrength; }
            set { localConf.SaberClashStrength = value; }
        }

        [UIValue("saber_clash_frequency")]
        public float SaberClashFrequency
        {
            get { return localConf.SaberClashFrequency; }
            set { localConf.SaberClashFrequency = value; }
        }

        [UIValue("ui_strength")]
        public float UIStrength
        {
            get { return localConf.UIStrength; }
            set { localConf.UIStrength = value; }
        }

        [UIValue("ui_frequency")]
        public float UIFrequency
        {
            get { return localConf.UIFrequency; }
            set { localConf.UIFrequency = value; }
        }

        [UIValue("ui_duration")]
        public float UIDuration
        {
            get { return localConf.UIDuration; }
            set { localConf.UIDuration = value; }
        }

        [UIAction("#post-parse")]
        internal void PostParse()
        {
        }

        [UIAction("#apply")]
        internal void OnApply()
        {
            conf.CopyFrom(localConf);
        }

        [UIAction("#cancel")]
        internal void OnCancel()
        {
            localConf.CopyFrom(conf);
        }

        [UIAction("rumble_test_wall")]
        internal void OnRumbleTestWall()
        {
            RumbleTest(localConf.WallStrength, localConf.WallFrequency, kContinuousRumbleTestDuration);
        }

        [UIAction("rumble_test_note")]
        internal void OnRumbleTestNote()
        {
            RumbleTest(localConf.NoteStrength, localConf.NoteFrequency, localConf.NoteDuration);
        }

        [UIAction("rumble_test_arc")]
        internal void OnRumbleTestArc()
        {
            RumbleTest(localConf.ArcStrength, localConf.ArcFrequency, kContinuousRumbleTestDuration);
        }

        [UIAction("rumble_test_saber_clash")]
        internal void OnRumbleTestSaberClash()
        {
            RumbleTest(localConf.SaberClashStrength, localConf.SaberClashFrequency, kContinuousRumbleTestDuration);
        }

        [UIAction("rumble_test_ui")]
        internal void OnRumbleTestUI()
        {
            RumbleTest(localConf.UIStrength, localConf.UIFrequency, localConf.UIDuration);
        }

        private void RumbleTest(float strength, float frequency, float duration)
        {
            var hands = new[] { CommonUsages.LeftHand, CommonUsages.RightHand };

            // for each of LeftHand and RightHand:
            foreach (var hand in hands)
            {
                // get the InputDevice of the controller
                var device = InputSystem.GetDevice<UnityEngine.InputSystem.XR.XRController>(hand);

                // get the path of a haptic control, assuming the path contains the string "haptic"
                var hapticControl = device?.allControls.FirstOrDefault(c => c.path.Contains("haptic"));

                // if found, send the impulse
                if (hapticControl != null)
                {
                    var action = new InputAction(name: "HapticFeedbackOverride", type: InputActionType.PassThrough);
                    action.AddBinding(hapticControl.path);
                    action.Enable();

                    OpenXRInput.SendHapticImpulse(action, strength, frequency, duration, device);
                }
            }
        }
    }

}
