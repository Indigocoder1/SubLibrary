using UnityEngine;

namespace SubLibrary.CyclopsReferencers;

internal class DamagePointSlot : MonoBehaviour
{
    // Code credit:
    // SealSub's DamagePointSlot https://github.com/32Kallies/SealSub/blob/acafaae6573625c695642439530c93c2280ebf03/SealSubMod/MonoBehaviours/Prefab/DamagePointSlot.cs
    // Contributors:
    // - Kallie23
    
    [Header("The specific child index of cyclops damage", order = 0), Space(-10, order = 1)]
    [Header("prefabs to use for this object.", order = 2), Space(-10, order = 3)]
    [Header("Use -1 to pick a random one.", order = 4), Space(-10, order = 5)]
    [Space]
    public int damagePrefabIndex = -1;
}
