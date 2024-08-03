using HarmonyLib;
using SubLibrary.Monobehaviors;

namespace SubLibrary.Patches;

[HarmonyPatch(typeof(VFXConstructing))]
internal class VFXConstructingPatches
{
    [HarmonyPatch(nameof(VFXConstructing.Construct)), HarmonyPostfix]
    private static void Construct_Postfix(VFXConstructing __instance)
    {
        if(__instance is CustomSubVFXConstructing customConstructing)
        {
            customConstructing.onConstructionStarted?.Invoke();
        }
    }

    [HarmonyPatch(nameof(VFXConstructing.EndConstruct)), HarmonyPostfix]
    private static void EndConstruct_Postfix(VFXConstructing __instance)
    {
        if (__instance is CustomSubVFXConstructing customConstructing)
        {
            customConstructing.onConstructionFinished?.Invoke();
        }
    }
}
