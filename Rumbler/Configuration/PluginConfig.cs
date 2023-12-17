using System.Runtime.CompilerServices;
using IPA.Config.Stores;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]
namespace Rumbler.Configuration
{
    internal class PluginConfig
    {
        public static PluginConfig Instance { get; set; }

        public virtual bool IsEnabled { get; set; } = false;

        // notes
        public virtual float NoteStrength { get; set; } = 0.5f;
        public virtual float NoteFrequency { get; set; } = 100f;
        public virtual float NoteDuration { get; set; } = 0.05f;

        // walls
        public virtual float WallStrength { get; set; } = 0.5f;
        public virtual float WallFrequency { get; set; } = 0.5f;

        // arcs
        public virtual float ArcStrength { get; set; } = 0.5f;
        public virtual float ArcFrequency { get; set; } = 100f;

        // saber clash
        public virtual float SaberClashStrength { get; set; } = 0.5f;
        public virtual float SaberClashFrequency { get; set; } = 0.5f;

        // UI
        public virtual float UIStrength { get; set; } = 0.5f;
        public virtual float UIFrequency { get; set; } = 100f;
        public virtual float UIDuration { get; set; } = 0.05f;

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
            // Do stuff when the config is changed.
        }

        /// <summary>
        /// Call this to have BSIPA copy the values from <paramref name="other"/> into this config.
        /// </summary>
        public virtual void CopyFrom(PluginConfig other)
        {
            IsEnabled = other.IsEnabled;
            NoteStrength = other.NoteStrength;
            NoteFrequency = other.NoteFrequency;
            NoteDuration = other.NoteDuration;
            WallStrength = other.WallStrength;
            WallFrequency = other.WallFrequency;
            ArcStrength = other.ArcStrength;
            ArcFrequency = other.ArcFrequency;
            SaberClashStrength = other.SaberClashStrength;
            SaberClashFrequency = other.SaberClashFrequency;
            UIStrength = other.UIStrength;
            UIFrequency = other.UIFrequency;
            UIDuration = other.UIDuration;
        }
    }
}
