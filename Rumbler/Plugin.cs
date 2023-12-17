using IPA;
using IPA.Config;
using IPA.Config.Stores;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using IPALogger = IPA.Logging.Logger;
using BeatSaberMarkupLanguage.Settings;
using HarmonyLib;

namespace Rumbler
{
    [Plugin(RuntimeOptions.SingleStartInit)]
    public class Plugin
    {
        internal static Plugin Instance { get; private set; }
        internal static IPALogger Log { get; private set; }

        private SettingsController settingsController;

        internal const string HarmonyId = "com.github.marekp0.Rumbler";
        internal static Harmony HarmonyInstance => new Harmony(HarmonyId);

        [Init]
        /// <summary>
        /// Called when the plugin is first loaded by IPA (either when the game starts or when the plugin is enabled if it starts disabled).
        /// [Init] methods that use a Constructor or called before regular methods like InitWithConfig.
        /// Only use [Init] with one Constructor.
        /// </summary>
        public void Init(IPALogger logger, Config conf)
        {
            Instance = this;
            Log = logger;
            Log.Info("Rumbler initialized.");

            // load config
            Configuration.PluginConfig.Instance = conf.Generated<Configuration.PluginConfig>();
            Log.Debug("Config loaded");

            // settings controller
            settingsController = new SettingsController(Configuration.PluginConfig.Instance);
        }

        [OnEnable]
        public void OnEnable()
        {
            BSMLSettings.instance.AddSettingsMenu("Rumbler", $"Rumbler.Views.Settings.bsml", settingsController);
            if (Configuration.PluginConfig.Instance.IsEnabled)
            {
                ApplyHarmonyPatches();
            }
        }

        [OnStart]
        public void OnApplicationStart()
        {
            Log.Debug("OnApplicationStart");
            new GameObject("RumblerController").AddComponent<RumblerController>();

        }

        [OnDisable]
        public void OnDisable()
        {
            BSMLSettings.instance.RemoveSettingsMenu(settingsController);
            RemoveHarmonyPatches();
        }

        [OnExit]
        public void OnApplicationQuit()
        {
            Log.Debug("OnApplicationQuit");

        }

        internal static void ApplyHarmonyPatches()
        {
            try
            {
                HarmonyInstance.PatchAll();
                Log?.Info("Successfully applied Harmony patches");
            }
            catch (Exception e)
            {
                Log?.Critical("Error adding Harmony patches: " + e.Message);
                Log?.Debug(e);
            }
        }

        internal static void RemoveHarmonyPatches()
        {
            try
            {
                HarmonyInstance.UnpatchSelf();
                Log?.Info("Successfully removed Harmony patches");
            }
            catch (Exception e)
            {
                Log?.Critical("Error removing Harmony patches: " + e.Message);
                Log?.Debug(e);
            }
        }
    }
}
