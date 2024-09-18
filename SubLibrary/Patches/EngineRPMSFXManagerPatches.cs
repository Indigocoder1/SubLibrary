using HarmonyLib;
using SubLibrary.Monobehaviors;

namespace SubLibrary.Patches;

[HarmonyPatch(typeof(EngineRpmSFXManager))]
internal class EngineRPMSFXManagerPatches
{
    [HarmonyPatch(nameof(EngineRpmSFXManager.Update)), HarmonyPrefix]
    private static bool Update_Prefix(EngineRpmSFXManager __instance)
    {
        if (__instance is not CustomEngineSFXManager manager) return true;

        manager.OverrideUpdate();

        return false;
    }
}
