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
using Rumbler.Configuration;
using HMUI;


namespace Rumbler
{
    [HotReload(RelativePathToLayout = @"Views/Settings.bsml")]
    [ViewDefinition("Rumbler.Views.Settings.bsml")]
    internal class SettingsController : BSMLAutomaticViewController
    {
        private PluginConfig conf;
        private PluginConfig localConf;

        private const float kContinuousRumbleTestDuration = 2f;

        // BSML will save 0 to properties in tabs that were never accessed,
        // so we need to keep track of accessed tabs and do nothing for
        // "set" in tabs that were not accessed.
        private bool[] tabAccessed = new bool[4];

        public SettingsController(PluginConfig conf)
        {
            this.conf = conf;
            localConf = new PluginConfig();
            localConf.CopyFrom(conf);
        }

        private void ResetTabAccessed()
        {
            Plugin.Log?.Info("Resetting tab accessed");
            for (int i = 0; i < tabAccessed.Length; i++)
            {
                tabAccessed[i] = i == 0;
            }
        }

        [UIAction("tab_changed")]
        public void OnTabChanged(SegmentedControl component, int tab)
        {
            tabAccessed[tab] = true;
        }

        [UIValue("enabled")]
        public bool IsEnabled {
            get { return localConf.IsEnabled; }
            set { if (tabAccessed[0]) localConf.IsEnabled = value; }
        }

        [UIValue("note_cut_normal_strength")]
        public float NoteStrength
        {
            get { return localConf.NoteCutNormal.Strength; }
            set { if (tabAccessed[1]) localConf.NoteCutNormal.Strength = value; }
        }

        [UIValue("note_cut_normal_frequency")]
        public float NoteFrequency
        {
            get { return localConf.NoteCutNormal.Frequency; }
            set { if (tabAccessed[1]) localConf.NoteCutNormal.Frequency = value; }
        }

        [UIValue("note_cut_normal_duration")]
        public float NoteDuration
        {
            get { return localConf.NoteCutNormal.Duration; }
            set { if (tabAccessed[1]) localConf.NoteCutNormal.Duration = value; }
        }

        [UIValue("note_cut_short_normal_strength")]
        public float NoteCutShortNormalStrength
        {
            get { return localConf.NoteCutShortNormal.Strength; }
            set { if (tabAccessed[1]) localConf.NoteCutShortNormal.Strength = value; }
        }

        [UIValue("note_cut_short_normal_frequency")]
        public float NoteCutShortNormalFrequency
        {
            get { return localConf.NoteCutShortNormal.Frequency; }
            set { if (tabAccessed[1]) localConf.NoteCutShortNormal.Frequency = value; }
        }

        [UIValue("note_cut_short_normal_duration")]
        public float NoteCutShortNormalDuration
        {
            get { return localConf.NoteCutShortNormal.Duration; }
            set { if (tabAccessed[1]) localConf.NoteCutShortNormal.Duration = value; }
        }

        [UIValue("note_cut_short_weak_strength")]
        public float NoteCutShortWeakStrength
        {
            get { return localConf.NoteCutShortWeak.Strength; }
            set { if (tabAccessed[1]) localConf.NoteCutShortWeak.Strength = value; }
        }

        [UIValue("note_cut_short_weak_frequency")]
        public float NoteCutShortWeakFrequency
        {
            get { return localConf.NoteCutShortWeak.Frequency; }
            set { if (tabAccessed[1]) localConf.NoteCutShortWeak.Frequency = value; }
        }

        [UIValue("note_cut_short_weak_duration")]
        public float NoteCutShortWeakDuration
        {
            get { return localConf.NoteCutShortWeak.Duration; }
            set { if (tabAccessed[1]) localConf.NoteCutShortWeak.Duration = value; }
        }

        [UIValue("note_cut_bomb_strength")]
        public float NoteCutBombStrength
        {
            get { return localConf.NoteCutBomb.Strength; }
            set { if (tabAccessed[1]) localConf.NoteCutBomb.Strength = value; }
        }

        [UIValue("note_cut_bomb_frequency")]
        public float NoteCutBombFrequency
        {
            get { return localConf.NoteCutBomb.Frequency; }
            set { if (tabAccessed[1]) localConf.NoteCutBomb.Frequency = value; }
        }

        [UIValue("note_cut_bomb_duration")]
        public float NoteCutBombDuration
        {
            get { return localConf.NoteCutBomb.Duration; }
            set { if (tabAccessed[1]) localConf.NoteCutBomb.Duration = value; }
        }

        [UIValue("note_cut_bad_cut_strength")]
        public float NoteCutBadCutStrength
        {
            get { return localConf.NoteCutBadCut.Strength; }
            set { if (tabAccessed[1]) localConf.NoteCutBadCut.Strength = value; }
        }

        [UIValue("note_cut_bad_cut_frequency")]
        public float NoteCutBadCutFrequency
        {
            get { return localConf.NoteCutBadCut.Frequency; }
            set { if (tabAccessed[1]) localConf.NoteCutBadCut.Frequency = value; }
        }

        [UIValue("note_cut_bad_cut_duration")]
        public float NoteCutBadCutDuration
        {
            get { return localConf.NoteCutBadCut.Duration; }
            set { if (tabAccessed[1]) localConf.NoteCutBadCut.Duration = value; }
        }

        [UIValue("obstacle_strength")]
        public float ObstacleStrength
        { 
            get { return localConf.Obstacle.Strength;}
            set { if (tabAccessed[2]) localConf.Obstacle.Strength = value; }
        }

        [UIValue("obstacle_frequency")]
        public float ObstacleFrequency
        {
            get { return localConf.Slider.Frequency; }
            set { if (tabAccessed[2]) localConf.Slider.Frequency = value; }
        }

        [UIValue("slider_strength")]
        public float ArcStrength
        {
            get { return localConf.Slider.Strength; }
            set { if (tabAccessed[2]) localConf.Slider.Strength = value; }
        }

        [UIValue("slider_frequency")]
        public float ArcFrequency
        {
            get { return localConf.Slider.Frequency; }
            set { if (tabAccessed[2]) localConf.Slider.Frequency = value; }
        }

        [UIValue("saber_clash_strength")]
        public float SaberClashStrength
        {
            get { return localConf.SaberClash.Strength; }
            set { if (tabAccessed[2]) localConf.SaberClash.Strength = value; }
        }

        [UIValue("saber_clash_frequency")]
        public float SaberClashFrequency
        {
            get { return localConf.SaberClash.Frequency; }
            set { if (tabAccessed[2]) localConf.SaberClash.Frequency = value; }
        }

        [UIValue("ui_strength")]
        public float UIStrength
        {
            get { return localConf.UI.Strength; }
            set { if (tabAccessed[3]) localConf.UI.Strength = value; }
        }

        [UIValue("ui_frequency")]
        public float UIFrequency
        {
            get { return localConf.UI.Frequency; }
            set { if (tabAccessed[3]) localConf.UI.Frequency = value; }
        }

        [UIValue("ui_duration")]
        public float UIDuration
        {
            get { return localConf.UI.Duration; }
            set { if (tabAccessed[3]) localConf.UI.Duration = value; }
        }

        [UIAction("#post-parse")]
        internal void PostParse()
        {
            ResetTabAccessed();
        }

        [UIAction("#apply")]
        internal void OnApply()
        {
            conf.CopyFrom(localConf);
            Plugin.OnConfigChanged();
        }

        [UIAction("#cancel")]
        internal void OnCancel()
        {
            localConf.CopyFrom(conf);
        }

        [UIAction("rumble_test_note_cut_normal")]
        internal void OnRumbleTestNoteCutNormal()
        {
            RumbleTest(localConf.NoteCutNormal);
        }

        [UIAction("rumble_test_note_cut_short_normal")]
        internal void OnRumbleTestNoteCutShortNormal()
        {
            RumbleTest(localConf.NoteCutShortNormal);
        }

        [UIAction("rumble_test_note_cut_short_weak")]
        internal void OnRumbleTestNoteCutShortWeak()
        {
            RumbleTest(localConf.NoteCutShortWeak);
        }

        [UIAction("rumble_test_note_cut_bomb")]
        internal void OnRumbleTestNoteCutBomb()
        {
            RumbleTest(localConf.NoteCutBomb);
        }

        [UIAction("rumble_test_note_cut_bad_cut")]
        internal void OnRumbleTestNoteCutBadCut()
        {
            RumbleTest(localConf.NoteCutBadCut);
        }

        [UIAction("rumble_test_obstacle")]
        internal void OnRumbleTestObstacle()
        {
            RumbleTest(localConf.Obstacle);
        }

        [UIAction("rumble_test_slider")]
        internal void OnRumbleTestArc()
        {
            RumbleTest(localConf.Slider);
        }

        [UIAction("rumble_test_saber_clash")]
        internal void OnRumbleTestSaberClash()
        {
            RumbleTest(localConf.SaberClash);
        }

        [UIAction("rumble_test_ui")]
        internal void OnRumbleTestUI()
        {
            RumbleTest(localConf.UI);
        }


        private void RumbleTest(RumbleParams rumbleParams)
        {
            var hands = new[] { CommonUsages.LeftHand, CommonUsages.RightHand };
            float duration = rumbleParams.Duration == 0f ? kContinuousRumbleTestDuration : rumbleParams.Duration;

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

                    OpenXRInput.SendHapticImpulse(action, rumbleParams.Strength, rumbleParams.Frequency, duration, device);
                }
            }
        }
    }

}
