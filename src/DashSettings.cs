using BepInEx;
using UnityEngine;
using System.IO;
using BepInEx.Configuration;

namespace DashMonke
{
    static public class DashSettings
    {
        static public Dash dash;
        static public bool UseRight;
        static public bool UsePrimary;
        static public void LoadSettings()
        {
            var File = new ConfigFile(Path.Combine(Paths.ConfigPath, "DashMonke.cfg"), true);

            ConfigEntry<float> CfgPower = File.Bind("Dash Settings", "Power", 15f);
            ConfigEntry<float> CfgEnd = File.Bind("Dash Settings", "EndPower", 17f);
            ConfigEntry<float> CfgDuration = File.Bind("Dash Settings", "Duration", 0.2f);

            ConfigEntry<bool> CfgControl = File.Bind("Dash Settings", "Controlled", false);
            ConfigEntry<bool> CfgCancel = File.Bind("Dash Settings", "UseCancel", false);

            ConfigEntry<float> CfgMP = File.Bind("Dash Settings", "Powermultiplier", 0f);
            ConfigEntry<float> CfgME = File.Bind("Dash Settings", "Endmultiplier", 0f);

            ConfigEntry<float> CFGwait = File.Bind("Dash Settings", "WaitTime", 0f);

            dash = new Dash(CfgPower.Value, CfgEnd.Value, CfgDuration.Value, CfgControl.Value, CfgCancel.Value, CfgMP.Value, CfgME.Value, CFGwait.Value);

            ConfigEntry<bool> CfgRight = File.Bind("Input Settings", "UseRight", true);
            ConfigEntry<bool> CfgPrimary = File.Bind("Input Settings", "UsePrimary", true);

            UseRight = CfgRight.Value;
            UsePrimary = CfgPrimary.Value;
        }
    }
}
