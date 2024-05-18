using Dalamud.Configuration;
using Dalamud.Plugin;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace Waypoints;

[Serializable]
public class Configuration : IPluginConfiguration
{
    public int Version { get; set; } = 0;

    public Dictionary<string, (uint, Vector3)> Waypoints = new();

    [NonSerialized]
    private DalamudPluginInterface? PluginInterface;

    public void Initialize(DalamudPluginInterface pluginInterface)
    {
        PluginInterface = pluginInterface;
    }

    public void Save()
    {
        PluginInterface!.SavePluginConfig(this);
    }
}
