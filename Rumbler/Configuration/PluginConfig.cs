using System.Runtime.CompilerServices;
using IPA.Config.Stores;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]
namespace Rumbler.Configuration
{
    internal class RumbleParams
    {
        public virtual float Strength { get; set; } = 1f;

        // continuous rumbles should have a duration of 0
        public virtual float RumbleDuration { get; set; } = 0.13f;

        // frequency of a single pulse, doesn't matter much if PulseDuration is low
        public virtual float PulseFrequency { get; set; } = 0f;

        // duration of a single pulse
        public virtual float PulseDuration { get; set; } = 0.004f;

        // time between the start of successive pulses
        public virtual float PulseTrainPeriod { get; set; } = 0.01f;

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
            rumble.rumbleDuration = RumbleDuration;
            rumble.pulseStrength = Strength;
            rumble.pulseFrequency = PulseFrequency;
            rumble.pulseDuration = PulseDuration;
            rumble.pulseTrainPeriod = PulseTrainPeriod;
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
        public virtual RumbleParams AllContinuous { get; set; } = new RumbleParams { RumbleDuration = 0f };
        public virtual RumbleParams Obstacle { get; set; } = new RumbleParams { RumbleDuration = 0f };
        public virtual RumbleParams Slider { get; set; } = new RumbleParams { RumbleDuration = 0f };
        public virtual RumbleParams SaberClash { get; set; } = new RumbleParams { RumbleDuration = 0f };

        // UI
        public virtual RumbleParams UI { get; set; } = new RumbleParams { RumbleDuration = 0.01f };

        /// <summary>
        /// This is called whenever BSIPA reads the config from disk (including when file changes are detected).
        /// </summary>
        public virtual void OnReload()
        {
            // Do stuff after config is read from disk.
        }

        /// <summary>
        /// Call this to force BSIPA to update the config file. This is also called by BSIPA if it detects the file was modified.
        /// </summary>
        public virtual void Changed()
        {
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
    }
}
