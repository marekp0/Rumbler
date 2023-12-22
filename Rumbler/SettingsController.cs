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

        [UIValue("rumble_duration_step")]
        private const int kRumbleDurationStep = 5;
        [UIValue("rumble_duration_min")]
        private const int kRumbleDurationMin = kRumbleDurationStep;
        [UIValue("rumble_duration_max")]
        private const int kRumbleDurationMax = 500;

        [UIValue("pulse_frequency_step")]
        private const int kPulseFrequencyStep = 1;
        [UIValue("pulse_frequency_min")]
        private const int kPulseFrequencyMin = 0;
        [UIValue("pulse_frequency_max")]
        private const int kPulseFrequencyMax = 300;

        [UIValue("pulse_duration_step")]
        private const int kPulseDurationStep = 1;
        [UIValue("pulse_duration_min")]
        private const int kPulseDurationMin = kPulseDurationStep;
        [UIValue("pulse_duration_max")]
        private const int kPulseDurationMax = kRumbleDurationMax;

        [UIValue("pulse_train_period_step")]
        private const int kPulseTrainPeriodStep = 5;
        [UIValue("pulse_train_period_min")]
        private const int kPulseTrainPeriodMin = kPulseTrainPeriodStep;
        [UIValue("pulse_train_period_max")]
        private const int kPulseTrainPeriodMax = kRumbleDurationMax;

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

        /// <summary>
        /// Returns the lowest multiple of <paramref name="multiple"/> that is greater than or equal to <paramref name="min"/>
        /// </summary>
        /// <param name="multiple"></param>
        /// <param name="min"></param>
        /// <returns></returns>
        static int LowestMultiple(int multiple, int min)
        {
            return (min + multiple - 1) / multiple * multiple;
        }

        /// <summary>
        /// Returns the highest multiple of <paramref name="multiple"/> that is less than or equal to <paramref name="max"/>
        /// </summary>
        /// <param name="multiple"></param>
        /// <param name="min"></param>
        /// <returns></returns>
        static int HighestMultiple(int multiple, int max)
        {
            return max / multiple * multiple;
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
        public List<object> NoteTypes => noteTypeParams.Keys.ToList<object>();

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
        public int NoteCutStrength
        {
            get { return currentNoteTypeParams.Strength; }
            set
            {
                currentNoteTypeParams.Strength = value;
                NoteCutAllSame = selectedNoteType == "All";
            }
        }

        [UIValue("note_cut_rumble_duration")]
        public int NoteCutRumbleDuration
        {
            get { return currentNoteTypeParams.RumbleDuration; }
            set
            {
                currentNoteTypeParams.RumbleDuration = value;
                NoteCutAllSame = selectedNoteType == "All";

                if (NoteCutPulseDuration > value)
                {
                    NoteCutPulseDuration = HighestMultiple(kPulseDurationStep, value);
                    NotifyPropertyChanged("NoteCutPulseDuration");
                }
                if (NoteCutPulseTrainPeriod > value)
                {
                    NoteCutPulseTrainPeriod = HighestMultiple(kPulseTrainPeriodStep, value);
                    NotifyPropertyChanged("NoteCutPulseTrainPeriod");
                }
            }
        }

        [UIValue("note_cut_pulse_frequency")]
        public int NoteCutPulseFrequency
        {
            get { return currentNoteTypeParams.PulseFrequency; }
            set
            {
                currentNoteTypeParams.PulseFrequency = value;
                NoteCutAllSame = selectedNoteType == "All";
            }
        }

        [UIValue("note_cut_pulse_duration")]
        public int NoteCutPulseDuration
        {
            get { return currentNoteTypeParams.PulseDuration; }
            set
            {
                currentNoteTypeParams.PulseDuration = value;
                NoteCutAllSame = selectedNoteType == "All";

                if (NoteCutRumbleDuration < value)
                {
                    NoteCutRumbleDuration = LowestMultiple(kRumbleDurationStep, value);
                    NotifyPropertyChanged("NoteCutRumbleDuration");
                }
                if (NoteCutPulseTrainPeriod < value)
                {
                    NoteCutPulseTrainPeriod = LowestMultiple(kPulseTrainPeriodStep, value);
                    NotifyPropertyChanged("NoteCutPulseTrainPeriod");
                }
            }
        }

        [UIValue("note_cut_pulse_train_period")]
        public int NoteCutPulseTrainPeriod
        {
            get { return currentNoteTypeParams.PulseTrainPeriod; }
            set
            {
                currentNoteTypeParams.PulseTrainPeriod = value;
                NoteCutAllSame = selectedNoteType == "All";
                NotifyPropertyChanged();

                if (NoteCutRumbleDuration < value)
                {
                    NoteCutRumbleDuration = LowestMultiple(kRumbleDurationStep, value);
                    NotifyPropertyChanged("NoteCutRumbleDuration");
                }
                if (NoteCutPulseDuration > value)
                {
                    NoteCutPulseDuration = HighestMultiple(kPulseDurationStep, value);
                    NotifyPropertyChanged("NoteCutPulseDuration");
                }
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
        public int ContinuousStrength
        {
            get { return currentContinuousTypeParams.Strength; }
            set {
                currentContinuousTypeParams.Strength = value;
                ContinuousAllSame = selectedContinuousType == "All";
            }
        }

        [UIValue("continuous_pulse_frequency")]
        public int ContinuousPulseFrequency
        {
            get { return currentContinuousTypeParams.PulseFrequency; }
            set {
                currentContinuousTypeParams.PulseFrequency = value;
                ContinuousAllSame = selectedContinuousType == "All";
            }
        }

        [UIValue("continuous_pulse_duration")]
        public int ContinuousPulseDuration
        {
            get { return currentContinuousTypeParams.PulseDuration; }
            set {
                currentContinuousTypeParams.PulseDuration = value;
                ContinuousAllSame = selectedContinuousType == "All";

                if (ContinuousPulseTrainPeriod < value)
                {
                    ContinuousPulseTrainPeriod = LowestMultiple(kPulseTrainPeriodStep, value);
                    NotifyPropertyChanged("ContinuousPulseTrainPeriod");
                }
            }
        }

        [UIValue("continuous_pulse_train_period")]
        public int ContinuousPulseTrainPeriod
        {
            get { return currentContinuousTypeParams.PulseTrainPeriod; }
            set {
                currentContinuousTypeParams.PulseTrainPeriod = value;
                ContinuousAllSame = selectedContinuousType == "All";

                if (ContinuousPulseDuration > value)
                {
                    ContinuousPulseDuration = HighestMultiple(kPulseDurationStep, value);
                    NotifyPropertyChanged("ContinuousPulseDuration");
                }
            }
        }

        [UIAction("rumble_test_continuous")]
        internal void OnRumbleTestContinuous()
        {
            RumbleTest(currentContinuousTypeParams);
        }

        #endregion

        #region UI

        [UIValue("ui_strength")]
        public int UIStrength
        {
            get { return localConf.UI.Strength; }
            set { localConf.UI.Strength = value; }
        }

        [UIValue("ui_rumble_duration")]
        public int UIRumbleDuration
        {
            get { return localConf.UI.RumbleDuration; }
            set
            {
                localConf.UI.RumbleDuration = value;

                if (UIPulseDuration > value)
                {
                    UIPulseDuration = HighestMultiple(kPulseDurationStep, value);
                    NotifyPropertyChanged("UIPulseDuration");
                }

                if (UIPulseTrainPeriod > value)
                {
                    UIPulseTrainPeriod = HighestMultiple(kPulseTrainPeriodStep, value);
                    NotifyPropertyChanged("UIPulseTrainPeriod");
                }
            }
        }

        [UIValue("ui_pulse_frequency")]
        public int UIPulseFrequency
        {
            get { return localConf.UI.PulseFrequency; }
            set { localConf.UI.PulseFrequency = value; }
        }

        [UIValue("ui_pulse_duration")]
        public int UIPulseDuration
        {
            get { return localConf.UI.PulseDuration; }
            set {
                localConf.UI.PulseDuration = value;

                if (UIRumbleDuration < value)
                {
                    UIRumbleDuration = LowestMultiple(kRumbleDurationStep, value);
                    NotifyPropertyChanged("UIRumbleDuration");
                }
                if (UIPulseTrainPeriod < value)
                {
                    UIPulseTrainPeriod = LowestMultiple(kPulseTrainPeriodStep, value);
                    NotifyPropertyChanged("UIPulseTrainPeriod");
                }
            }
        }

        [UIValue("ui_pulse_train_period")]
        public int UIPulseTrainPeriod
        {
            get { return localConf.UI.PulseTrainPeriod; }
            set
            {
                localConf.UI.PulseTrainPeriod = value;

                if (UIRumbleDuration < value)
                {
                    UIRumbleDuration = LowestMultiple(kRumbleDurationStep, value);
                    NotifyPropertyChanged("UIRumbleDuration");
                }
                if (UIPulseDuration > value)
                {
                    UIPulseDuration = HighestMultiple(kPulseDurationStep, value);
                    NotifyPropertyChanged("UIPulseDuration");
                }
            }
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
                    NotifyPropertyChanged(prop.Name);
                }
            }
        }

        private void RumbleTest(RumbleParams rumbleParams)
        {
            RumbleInfo rumble = rumbleParams.ToRumbleInfo();
            if (rumbleParams.RumbleDuration == 0)
            {
                rumble.rumbleDuration = kContinuousRumbleTestDuration;
            }

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
