using UnityEngine;

namespace MapForge.API
{
    public class MapForgeLog
    {
        public static void Info(object message)
        {
            Debug.Log($"<color=grey><b>[</b><color=cyan>MapForge</color><b>]</b></color> {message}");
        }
    }
}
