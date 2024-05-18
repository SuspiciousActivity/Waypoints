using System;
using System.Numerics;
using Dalamud.Interface.Windowing;
using ImGuiNET;

namespace Waypoints;

public class MainWindow : Window, IDisposable
{
    private string newName = string.Empty;

    public MainWindow()
        : base("Waypoints")
    {
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(375, 330),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
        };
    }

    public void Dispose() { }

    public override void Draw()
    {
        ImGui.InputText("", ref newName, 128);
        ImGui.SameLine();
        if (ImGui.Button("Add Waypoint"))
        {
            if (newName.Length == 0)
            {
                return;
            }
            Plugin.Configuration.Waypoints.Add(newName, (Utils.CurrentMapId(), Utils.CurrentPosition() ?? Vector3.Zero));
            Plugin.WotsitIpc.Update();
            newName = string.Empty;
        }

        ImGui.Spacing();

        ImGui.Text("Waypoints");
        foreach (var kvp in Plugin.Configuration.Waypoints)
        {
            if (ImGui.Button(kvp.Key))
            {
                Utils.OpenMapAt(kvp.Value.Item1, kvp.Value.Item2);
            }
            ImGui.SameLine();
            if (ImGui.Button($"Delete##{kvp.Key}"))
            {
                Plugin.Configuration.Waypoints.Remove(kvp.Key);
                Plugin.WotsitIpc.Update();
            }
        }
    }
}
