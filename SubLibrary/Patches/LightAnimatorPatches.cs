using HarmonyLib;
using UnityEngine;

namespace SubLibrary.Patches;

[HarmonyPatch(typeof(LightAnimator))]
internal class LightAnimatorPatches
{
    [HarmonyPatch(nameof(LightAnimator.Start)), HarmonyPostfix]
    private static void Start_Postfix(LightAnimator __instance)
    {
        if (!__instance.lightComponent)
        {
            __instance.lightComponent = __instance.GetComponent<Light>();
        }
    }
}
