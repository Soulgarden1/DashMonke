using BepInEx;
using System;
using UnityEngine;
using Utilla;

namespace DashMonke
{
    /// <summary>
    /// This is your mod's main class.
    /// </summary>

    /* This attribute tells Utilla to look for [ModdedGameJoin] and [ModdedGameLeave] */
    [ModdedGamemode]
    [BepInDependency("org.legoandmars.gorillatag.utilla", "1.5.0")]
    [BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
    public class DashPlugin : BaseUnityPlugin
    {
        private DashManager DM;
        void OnEnable()
        {
            DashSettings.LoadSettings();
            if (DM != null) DM.enabled = true;

            HarmonyPatches.ApplyHarmonyPatches();
            Utilla.Events.GameInitialized += OnGameInitialized;
        }

        void OnDisable()
        {
            if (DM != null) DM.enabled = false;

            HarmonyPatches.RemoveHarmonyPatches();
            Utilla.Events.GameInitialized -= OnGameInitialized;
        }

        void OnGameInitialized(object sender, EventArgs e)
        {
            /* Code here runs after the game initializes (i.e. GorillaLocomotion.Player.Instance != null) */
        }

        /* This attribute tells Utilla to call this method when a modded room is joined */
        [ModdedGamemodeJoin]
        public void OnJoin(string gamemode)
        {
            DM = GorillaLocomotion.Player.Instance.gameObject.AddComponent<DashManager>();
            DM.enabled = this.enabled;
            DM.Pdash = DashSettings.dash;
        }
        
        /* This attribute tells Utilla to call this method when a modded room is left */
        [ModdedGamemodeLeave]
        public void OnLeave(string gamemode)
        {
            Destroy(DM);
        }
    }
}
