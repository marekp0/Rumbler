using System.Runtime.CompilerServices;
using IPA.Config.Stores;
using Libraries.HM.HMLib.VR;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]
namespace Rumbler.Configuration
{
    internal class RumbleParams
    {
        public virtual float Strength { get; set; } = 1f;
        public virtual float Frequency { get; set; } = 50f;

        // if this is 0, then the rumble will be continuous
        public virtual float Duration { get; set; } = 0.13f;

        public void CopyFrom(RumbleParams other)
        {
            Strength = other.Strength;
            Frequency = other.Frequency;
            Duration = other.Duration;
        }

        /// <summary>
        /// Copies the rumble parameters to a <c>HapticPresetSO</c> object.
        /// </summary>
        /// <param name="preset"><c>HapticPresetSO</c> object to copy to. Can be null.</param>
        public void CopyTo(HapticPresetSO preset)
        {
            if (preset == null) return;

            preset._strength = Strength;
            preset._frequency = Frequency;
            preset._duration = Duration;
            preset._continuous = Duration == 0f;
        }
    }

    internal class PluginConfig
    {
        public static PluginConfig Instance { get; set; }

        public virtual bool IsEnabled { get; set; } = false;

        public virtual RumbleParams NoteCutNormal { get; set; } = new RumbleParams();
        public virtual RumbleParams NoteCutShortNormal { get; set; } = new RumbleParams();
        public virtual RumbleParams NoteCutShortWeak { get; set; } = new RumbleParams();
        public virtual RumbleParams NoteCutBomb { get; set; } = new RumbleParams();
        public virtual RumbleParams NoteCutBadCut { get; set; } = new RumbleParams();
        public virtual RumbleParams Obstacle { get; set; } = new RumbleParams { Duration = 0f };
        public virtual RumbleParams Slider { get; set; } = new RumbleParams { Duration = 0f };
        public virtual RumbleParams SaberClash { get; set; } = new RumbleParams { Duration = 0f };
        public virtual RumbleParams UI { get; set; } = new RumbleParams();

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
            NoteCutNormal.CopyFrom(other.NoteCutNormal);
            NoteCutShortNormal.CopyFrom(other.NoteCutShortNormal);
            NoteCutShortWeak.CopyFrom(other.NoteCutShortWeak);
            NoteCutBomb.CopyFrom(other.NoteCutBomb);
            NoteCutBadCut.CopyFrom(other.NoteCutBadCut);
            Obstacle.CopyFrom(other.Obstacle);
            Slider.CopyFrom(other.Slider);
            SaberClash.CopyFrom(other.SaberClash);
            UI.CopyFrom(other.UI);
        }
    }
}
