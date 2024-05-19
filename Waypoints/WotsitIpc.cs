using System;
using System.Collections.Generic;
using System.Numerics;
using System.Timers;
using Dalamud.Plugin.Ipc;

namespace Waypoints
{
    public class WotsitIpc
    {
        private ICallGateSubscriber<string, string, string, uint, string> _registerWithSearch;
        private ICallGateSubscriber<string, bool> _invoke;
        private ICallGateSubscriber<string, bool> _unregisterAll;
        private Timer _timer;

        private Dictionary<string, (uint, Vector3)> _map = new();

        public WotsitIpc()
        {
            _registerWithSearch = Plugin.PluginInterface.GetIpcSubscriber<string, string, string, uint, string>("FA.RegisterWithSearch");
            _unregisterAll = Plugin.PluginInterface.GetIpcSubscriber<string, bool>("FA.UnregisterAll");

            _invoke = Plugin.PluginInterface.GetIpcSubscriber<string, bool>("FA.Invoke");
            _invoke.Subscribe(Invoke);
            _timer = new Timer(5000);
            _timer.Elapsed += (sender, e) => RetryInit();
            if (!Update())
            {
                _timer.Start();
            }
        }

        private void RetryInit()
        {
            _registerWithSearch = Plugin.PluginInterface.GetIpcSubscriber<string, string, string, uint, string>("FA.RegisterWithSearch");
            _unregisterAll = Plugin.PluginInterface.GetIpcSubscriber<string, bool>("FA.UnregisterAll");

            _invoke = Plugin.PluginInterface.GetIpcSubscriber<string, bool>("FA.Invoke");
            _invoke.Subscribe(Invoke);
            if (Update())
            {
                _timer.Stop();
            }
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);

            UnregisterAll();
        }

        public bool Update()
        {
            _map.Clear();
            if (!UnregisterAll())
            {
                return false;
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
            return true;
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
