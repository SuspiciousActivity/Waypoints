using Dalamud.Interface.Windowing;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;

namespace Waypoints;

public sealed class Plugin : IDalamudPlugin
{
    [PluginService] public static IDalamudPluginInterface PluginInterface { get; private set; } = null!;
    [PluginService] public static IClientState ClientState { get; private set; } = null!;
    public static Configuration Configuration { get; private set; } = null!;
    public readonly WindowSystem WindowSystem = new("Waypoints");
    public static WotsitIpc WotsitIpc { get; private set; } = null!;
    public static MainWindow MainWindow { get; private set; } = null!;

    public Plugin()
    {
        Configuration = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
        Configuration.Initialize(PluginInterface);

        WotsitIpc = new();

        MainWindow = new();
        WindowSystem.AddWindow(MainWindow);
        PluginInterface.UiBuilder.Draw += WindowSystem.Draw;
        PluginInterface.UiBuilder.OpenMainUi += MainWindow.Toggle;
    }

    public void Dispose()
    {
        Configuration.Save();
        WindowSystem.RemoveAllWindows();
        WotsitIpc.Dispose();
    }
}
