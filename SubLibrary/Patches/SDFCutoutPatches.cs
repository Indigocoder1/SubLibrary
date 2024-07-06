using HarmonyLib;
using SubLibrary.CyclopsReferencers;
namespace SubLibrary.Patches;

[HarmonyPatch(typeof(SDFCutout))]
internal class SDFCutoutPatches
{
    [HarmonyPatch(nameof(SDFCutout.Start), MethodType.Enumerator), HarmonyPrefix]
    private static bool Start_Prefix(object __instance)
    {
        var fields = __instance.GetType().GetFields();

        SDFCutout cutout = (SDFCutout)fields[0].GetValue(__instance);
        var distanceFieldManager = cutout.GetComponent<DistanceFieldAssigner>();

        return distanceFieldManager == null;
    }
}
