using System;
using System.Collections.Generic;
using System.Numerics;
using Dalamud.Plugin.Ipc;

namespace Waypoints
{
    public class WotsitIpc
    {
        private readonly ICallGateSubscriber<string, string, string, uint, string> _registerWithSearch;
        private readonly ICallGateSubscriber<string, bool> _invoke;
        private readonly ICallGateSubscriber<string, bool> _unregisterAll;

        private Dictionary<string, (uint, Vector3)> _map = new();

        public WotsitIpc()
        {
            _registerWithSearch = Plugin.PluginInterface.GetIpcSubscriber<string, string, string, uint, string>("FA.RegisterWithSearch");
            _unregisterAll = Plugin.PluginInterface.GetIpcSubscriber<string, bool>("FA.UnregisterAll");

            _invoke = Plugin.PluginInterface.GetIpcSubscriber<string, bool>("FA.Invoke");
            _invoke.Subscribe(Invoke);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);

            UnregisterAll();
        }

        public void Update()
        {
            _map.Clear();
            if (!UnregisterAll())
            {
                return;
            }

            foreach (var kvp in Plugin.Configuration.Waypoints)
            {
                string guid = _registerWithSearch.InvokeFunc(
                    Plugin.PluginInterface.InternalName,
                    $"Waypoint: {kvp.Key}",
                    $"Waypoint: {kvp.Key}",
                    66472
                );

                _map.Add(guid, (kvp.Value.Item1, kvp.Value.Item2));
            }
        }

        public void Invoke(string guid)
        {
            if (!_map.TryGetValue(guid, out var value))
            {
                return;
            }
            Utils.OpenMapAt(value.Item1, value.Item2);
        }

        public bool UnregisterAll()
        {
            try
            {
                _unregisterAll.InvokeFunc(Plugin.PluginInterface.InternalName);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
