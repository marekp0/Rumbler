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
using BeatSaberMarkupLanguage.Components.Settings;


namespace Rumbler
{
    [HotReload(RelativePathToLayout = @"Views/Settings.bsml")]
    [ViewDefinition("Rumbler.Views.Settings.bsml")]
    internal class SettingsController : BSMLAutomaticViewController
    {
        private PluginConfig conf;
        private PluginConfig localConf = new PluginConfig();

        private const float kContinuousRumbleTestDuration = 2f;

        public SettingsController(PluginConfig conf)
        {
            this.conf = conf;
            localConf.CopyFrom(conf);

            noteTypeParams.Add("All", localConf.AllNoteCut);
            noteTypeParams.Add("Normal", localConf.NoteCutNormal);
            noteTypeParams.Add("Short normal", localConf.NoteCutShortNormal);
            noteTypeParams.Add("Short weak", localConf.NoteCutShortWeak);
            noteTypeParams.Add("Bomb", localConf.NoteCutBomb);
            noteTypeParams.Add("Bad cut", localConf.NoteCutBadCut);
            SelectedNoteType = localConf.NoteCutAllSame ? "All" : "Normal";

            continuousTypeParams.Add("All", localConf.AllContinuous);
            continuousTypeParams.Add("Obstacles (walls)", localConf.Obstacle);
            continuousTypeParams.Add("Sliders (arcs)", localConf.Slider);
            continuousTypeParams.Add("Saber clash", localConf.SaberClash);
            SelectedContinuousType = localConf.ContinuousAllSame ? "All" : "Obstacles (walls)";

            // redundant, but need to ensure this is called at least once before the
            // user ever presses OK; otherwise values of 0 will be set for any tabs
            // that the user never opened
            SyncControls();
        }

        #region General

        [UIValue("enabled")]
        public bool IsEnabled {
            get { return localConf.IsEnabled; }
            set { localConf.IsEnabled = value; }
        }

        [UIAction("default_settings_all")]
        internal void OnDefaultSettingsAll()
        {
            localConf.CopyFrom(new PluginConfig());
            SyncControls();
        }

        #endregion

        #region Note Cut

        private RumbleParams currentNoteTypeParams;
        private readonly Dictionary<string, RumbleParams> noteTypeParams = new Dictionary<string, RumbleParams>();

        [UIValue("note_types")]
        public List<Object> NoteTypes => noteTypeParams.Keys.ToList<object>();

        private string selectedNoteType = "All";

        [UIValue("selected_note_type")]
        public string SelectedNoteType
        {
            get { return selectedNoteType; }
            set
            {
                selectedNoteType = value;
                currentNoteTypeParams = noteTypeParams[value];
                SyncControls();
            }
        }

        [UIValue("note_cut_all_same")]
        public bool NoteCutAllSame
        {
            get { return localConf.NoteCutAllSame; }
            set
            {
                localConf.NoteCutAllSame = value;
                if (value) selectedNoteType = "All";
                NotifyPropertyChanged();
            }
        }

        [UIValue("note_cut_strength")]
        public float NoteCutStrength
        {
            get { return currentNoteTypeParams.Strength; }
            set
            {
                currentNoteTypeParams.Strength = value;
                NoteCutAllSame = selectedNoteType == "All";
            }
        }

        [UIValue("note_cut_rumble_duration")]
        public float NoteCutRumbleDuration
        {
            get { return currentNoteTypeParams.RumbleDuration; }
            set
            {
                currentNoteTypeParams.RumbleDuration = value;
                NoteCutAllSame = selectedNoteType == "All";
            }
        }

        [UIValue("note_cut_pulse_frequency")]
        public int NoteCutPulseFrequency
        {
            get { return (int)currentNoteTypeParams.PulseFrequency; }
            set
            {
                currentNoteTypeParams.PulseFrequency = value;
                NoteCutAllSame = selectedNoteType == "All";
            }
        }

        [UIValue("note_cut_pulse_duration")]
        public int NoteCutPulseDuration
        {
            // note: this is in seconds, but the UI shows milliseconds
            get { return (int)(currentNoteTypeParams.PulseDuration * 1000f); }
            set
            {
                currentNoteTypeParams.PulseDuration = value / 1000f;
                NoteCutAllSame = selectedNoteType == "All";
            }
        }

        [UIValue("note_cut_pulse_train_period")]
        public float NoteCutPulseTrainPeriod
        {
            get { return currentNoteTypeParams.PulseTrainPeriod; }
            set
            {
                currentNoteTypeParams.PulseTrainPeriod = value;
                NoteCutAllSame = selectedNoteType == "All";
            }
        }

        [UIAction("rumble_test_note_cut")]
        internal void OnRumbleTestNoteCut()
        {
            RumbleTest(currentNoteTypeParams);
        }

        #endregion

        #region Continuous

        private RumbleParams currentContinuousTypeParams;
        private readonly Dictionary<string, RumbleParams> continuousTypeParams = new Dictionary<string, RumbleParams>();

        [UIValue("continuous_types")]
        public List<object> ContinuousTypes => continuousTypeParams.Keys.ToList<object>();

        private string selectedContinuousType = "All";

        [UIValue("selected_continuous_type")]
        public string SelectedContinuousType
        {
            get { return selectedContinuousType; }
            set
            {
                selectedContinuousType = value;
                currentContinuousTypeParams = continuousTypeParams[value];
                SyncControls();
            }
        }

        [UIValue("continuous_all_same")]
        public bool ContinuousAllSame
        {
            get { return localConf.ContinuousAllSame; }
            set
            {
                localConf.ContinuousAllSame = value;
                if (value)  selectedContinuousType = "All";
                NotifyPropertyChanged();
            }
        }

        [UIValue("continuous_strength")]
        public float ContinuousStrength
        {
            get { return currentContinuousTypeParams.Strength; }
            set { currentContinuousTypeParams.Strength = value; }
        }

        [UIValue("continuous_pulse_frequency")]
        public int ContinuousPulseFrequency
        {
            get { return (int)currentContinuousTypeParams.PulseFrequency; }
            set { currentContinuousTypeParams.PulseFrequency = value; }
        }

        [UIValue("continuous_pulse_duration")]
        public int ContinuousPulseDuration
        {
            // note: this is in seconds, but the UI shows milliseconds
            get { return (int)(currentContinuousTypeParams.PulseDuration * 1000f); }
            set { currentContinuousTypeParams.PulseDuration = value / 1000f; }
        }

        [UIValue("continuous_pulse_train_period")]
        public float ContinuousPulseTrainPeriod
        {
            get { return currentContinuousTypeParams.PulseTrainPeriod; }
            set { currentContinuousTypeParams.PulseTrainPeriod = value; }
        }

        [UIAction("rumble_test_continuous")]
        internal void OnRumbleTestContinuous()
        {
            RumbleTest(currentContinuousTypeParams);
        }

        #endregion

        #region UI

        [UIValue("ui_strength")]
        public float UIStrength
        {
            get { return localConf.UI.Strength; }
            set { localConf.UI.Strength = value; }
        }

        [UIValue("ui_rumble_duration")]
        public float UIRumbleDuration
        {
            get { return localConf.UI.RumbleDuration; }
            set { localConf.UI.RumbleDuration = value; }
        }

        [UIValue("ui_pulse_frequency")]
        public int UIPulseFrequency
        {
            get { return (int)localConf.UI.PulseFrequency; }
            set { localConf.UI.PulseFrequency = value; }
        }

        [UIValue("ui_pulse_duration")]
        public int UIPulseDuration
        {
            // note: this is in seconds, but the UI shows milliseconds
            get { return (int)(localConf.UI.PulseDuration * 1000f); }
            set { localConf.UI.PulseDuration = value / 1000f; }
        }

        [UIValue("ui_pulse_train_period")]
        public float UIPulseTrainPeriod
        {
            get { return localConf.UI.PulseTrainPeriod; }
            set { localConf.UI.PulseTrainPeriod = value; }
        }

        [UIAction("rumble_test_ui")]
        internal void OnRumbleTestUI()
        {
            RumbleTest(localConf.UI);
        }

        #endregion


        [UIAction("#post-parse")]
        internal void PostParse()
        {
        }

        [UIAction("#apply")]
        internal void OnApply()
        {
            localConf.SyncAllSameSettings();
            conf.CopyFrom(localConf);
            Plugin.OnConfigChanged();
        }

        [UIAction("#cancel")]
        internal void OnCancel()
        {
            localConf.CopyFrom(conf);
        }

        /// <summary>
        /// Sets the value of all controls to reflect the current values in localConf
        /// </summary>
        private void SyncControls()
        {
            // for each property with a UIValue attribute, call NotifyPropertyChanged
            foreach (var prop in GetType().GetProperties())
            {
                if (prop.GetCustomAttributes(typeof(UIValue), true).FirstOrDefault() is UIValue)
                {
                    Plugin.Log?.Info($"Syncing control {prop.Name}");
                    NotifyPropertyChanged(prop.Name);
                }
            }
        }

        private void RumbleTest(RumbleParams rumbleParams)
        {
            RumbleInfo rumble;
            rumble.rumbleDuration = rumbleParams.RumbleDuration == 0 ? kContinuousRumbleTestDuration : rumbleParams.RumbleDuration;
            rumble.pulseStrength = rumbleParams.Strength;
            rumble.pulseFrequency = rumbleParams.PulseFrequency;
            rumble.pulseDuration = rumbleParams.PulseDuration;
            rumble.pulseTrainPeriod = rumbleParams.PulseTrainPeriod;

            var hapticFeedbackPlayer = RumblerController.Instance.HapticFeedbackPlayer;

            // stop any current haptics
            hapticFeedbackPlayer.StopHaptics(UnityEngine.XR.XRNode.LeftHand);
            hapticFeedbackPlayer.StopHaptics(UnityEngine.XR.XRNode.RightHand);

            // play haptics
            hapticFeedbackPlayer.PlayHapticFeedback(UnityEngine.XR.XRNode.LeftHand, rumble);
            hapticFeedbackPlayer.PlayHapticFeedback(UnityEngine.XR.XRNode.RightHand, rumble);
        }
    }

}
