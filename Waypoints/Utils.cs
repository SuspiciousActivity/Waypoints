using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using System.Numerics;

namespace Waypoints
{
    public static class Utils
    {
        public static unsafe uint CurrentMapId()
        {
            return AgentMap.Instance()->CurrentMapId;
        }

        public static Vector3? CurrentPosition()
        {
            return Plugin.ClientState.LocalPlayer?.Position;
        }

        public static unsafe void OpenMapAt(uint mapId, Vector3 position)
        {
            AgentMap.Instance()->SetFlagMapMarker(0, mapId, position);
            AgentMap.Instance()->OpenMap(mapId);
        }
    }
}
