using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.ViewControllers;
using IPA.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace Rumbler
{
    [HotReload(RelativePathToLayout = @"Views/Settings.bsml")]
    [ViewDefinition("Rumbler.Views.Settings.bsml")]
    internal class SettingsController : BSMLAutomaticViewController
    {
        private Configuration.PluginConfig conf;
        private Configuration.PluginConfig localConf;


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
    }
}
