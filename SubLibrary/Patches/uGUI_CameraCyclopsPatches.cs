using HarmonyLib;
using SubLibrary.Monobehaviors;

namespace SubLibrary.Patches;

[HarmonyPatch(typeof(uGUI_CameraCyclops))]
internal class uGUI_CameraCyclopsPatches
{
    [HarmonyPatch(nameof(uGUI_CameraCyclops.SetCamera)), HarmonyPostfix]
    private static void SetCamera_Postfix(uGUI_CameraCyclops __instance)
    {
        var nameAssigner = Player.main.GetCurrentSub().GetComponentInChildren<CameraNameAssigner>(true);
        if (!nameAssigner) return;

        __instance.textTitle.text = string.Empty;

        if (__instance.cameraIndex >= 0 && __instance.cameraIndex < nameAssigner.cameraNames.Length)
        {
            __instance.textTitle.text = Language.main.Get(nameAssigner.cameraNames[__instance.cameraIndex]);
        }
    }
}
