using AdminToys;
using HarmonyLib;

namespace MapForge.Patches
{
    [HarmonyPatch(typeof(AdminToyBase))]
    public static class ToyScalePatch
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(AdminToyBase.UpdatePositionServer))]
        public static bool OnUpdatePositionServer(AdminToyBase __instance)
        {
            if (__instance.IsStatic)
                return false;

            __instance.NetworkPosition = __instance.transform.position;
            __instance.NetworkRotation = __instance.transform.rotation;
            __instance.NetworkScale = __instance.transform.CalculateGlobalScale();
            return false;
        }
    }
}
