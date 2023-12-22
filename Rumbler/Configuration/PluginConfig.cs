using System.Runtime.CompilerServices;
using IPA.Config.Stores;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]
namespace Rumbler.Configuration
{
    internal class RumbleParams
    {
        // all durations in milliseconds

        // strength in percent, 0-100
        public virtual int Strength { get; set; } = 100;

        // continuous rumbles should have a duration of 0
        public virtual int RumbleDuration { get; set; } = 130;

        // frequency of a single pulse, doesn't matter much if PulseDuration is low
        public virtual int PulseFrequency { get; set; } = 0;

        // duration of a single pulse
        public virtual int PulseDuration { get; set; } = 4;

        // time between the start of successive pulses
        public virtual int PulseTrainPeriod { get; set; } = 10;

        public void CopyFrom(RumbleParams other)
        {
            Strength = other.Strength;
            RumbleDuration = other.RumbleDuration;
            PulseFrequency = other.PulseFrequency;
            PulseDuration = other.PulseDuration;
            PulseTrainPeriod = other.PulseTrainPeriod;
        }

        public RumbleInfo ToRumbleInfo()
        {
            RumbleInfo rumble;
            rumble.rumbleDuration = RumbleDuration/1000f;
            rumble.pulseStrength = Strength/100f;
            rumble.pulseFrequency = PulseFrequency/1000f;
            rumble.pulseDuration = PulseDuration / 1000f;
            rumble.pulseTrainPeriod = PulseTrainPeriod / 1000f;
            return rumble;
        }
    }

    internal class PluginConfig
    {
        public static PluginConfig Instance { get; set; }

        // general
        public virtual bool IsEnabled { get; set; } = false;

        // note cuts
        public virtual bool NoteCutAllSame { get; set; } = true;
        public virtual RumbleParams AllNoteCut { get; set; } = new RumbleParams();
        public virtual RumbleParams NoteCutNormal { get; set; } = new RumbleParams();
        public virtual RumbleParams NoteCutShortNormal { get; set; } = new RumbleParams();
        public virtual RumbleParams NoteCutShortWeak { get; set; } = new RumbleParams();
        public virtual RumbleParams NoteCutBomb { get; set; } = new RumbleParams();
        public virtual RumbleParams NoteCutBadCut { get; set; } = new RumbleParams();

        // continuous
        public virtual bool ContinuousAllSame { get; set; } = true;
        public virtual RumbleParams AllContinuous { get; set; } = new RumbleParams { RumbleDuration = 0 };
        public virtual RumbleParams Obstacle { get; set; } = new RumbleParams { RumbleDuration = 0 };
        public virtual RumbleParams Slider { get; set; } = new RumbleParams { RumbleDuration = 0 };
        public virtual RumbleParams SaberClash { get; set; } = new RumbleParams { RumbleDuration = 0 };

        // UI
        public virtual RumbleParams UI { get; set; } = new RumbleParams { RumbleDuration = 10 };

        /// <summary>
        /// This is called whenever BSIPA reads the config from disk (including when file changes are detected).
        /// </summary>
        public virtual void OnReload()
        {
            SyncAllSameSettings();
        }

        /// <summary>
        /// Call this to force BSIPA to update the config file. This is also called by BSIPA if it detects the file was modified.
        /// </summary>
        public virtual void Changed()
        {
            //SyncAllSameSettings();    // causes crash
            Plugin.OnConfigChanged();
        }

        /// <summary>
        /// Call this to have BSIPA copy the values from <paramref name="other"/> into this config.
        /// </summary>
        public virtual void CopyFrom(PluginConfig other)
        {
            IsEnabled = other.IsEnabled;

            NoteCutAllSame = other.NoteCutAllSame;
            AllNoteCut.CopyFrom(other.AllNoteCut);
            NoteCutNormal.CopyFrom(other.NoteCutNormal);
            NoteCutShortNormal.CopyFrom(other.NoteCutShortNormal);
            NoteCutShortWeak.CopyFrom(other.NoteCutShortWeak);
            NoteCutBomb.CopyFrom(other.NoteCutBomb);
            NoteCutBadCut.CopyFrom(other.NoteCutBadCut);

            ContinuousAllSame = other.ContinuousAllSame;
            AllContinuous.CopyFrom(other.AllContinuous);
            Obstacle.CopyFrom(other.Obstacle);
            Slider.CopyFrom(other.Slider);
            SaberClash.CopyFrom(other.SaberClash);

            UI.CopyFrom(other.UI);
        }

        /// <summary>
        /// Ensures that if one of the "all same" options is on, the individual RumbleParams
        /// are copied from the "all" RumbleParams.
        /// </summary>
        public void SyncAllSameSettings()
        {
            if (NoteCutAllSame)
            {
                NoteCutNormal.CopyFrom(AllNoteCut);
                NoteCutShortNormal.CopyFrom(AllNoteCut);
                NoteCutShortWeak.CopyFrom(AllNoteCut);
                NoteCutBomb.CopyFrom(AllNoteCut);
                NoteCutBadCut.CopyFrom(AllNoteCut);
            }

            if (ContinuousAllSame)
            {
                Obstacle.CopyFrom(AllContinuous);
                Slider.CopyFrom(AllContinuous);
                SaberClash.CopyFrom(AllContinuous);
            }
        }
    }
}
