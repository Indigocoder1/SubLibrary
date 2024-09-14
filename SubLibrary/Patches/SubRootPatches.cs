using HarmonyLib;
using SubLibrary.Monobehaviors;
using UnityEngine;

namespace SubLibrary.Patches;

[HarmonyPatch(typeof(SubRoot))]
internal class SubRootPatches
{
    private static bool OnCollisionEnter_Prefix(SubRoot __instance, Collision col)
    {
        var removeBangs = __instance.GetComponent<RemoveBangsFromSmallFish>();
        if (removeBangs == null) return true;

        if (col.gameObject.layer == LayerID.TerrainCollider) return true;

        LiveMixin mixin = col.gameObject.GetComponent<LiveMixin>();
        if (mixin == null) mixin = Utils.FindAncestorWithComponent<LiveMixin>(col.gameObject);

        if (mixin == null) return true;

        Rigidbody rigidbody = Utils.FindAncestorWithComponent<Rigidbody>(col.gameObject);

        if (rigidbody == null) return true;

        return rigidbody.mass > removeBangs.minMassForSound;
    }
}
