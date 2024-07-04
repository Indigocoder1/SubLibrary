using UnityEngine;

namespace SubLibrary.RuntimeSpawners;

internal class DamagePointSlot : MonoBehaviour
{
    [Header("The specific child index of cyclops damage", order = 0), Space(-10, order = 1)]
    [Header("prefabs to use for this object.", order = 2), Space(-10, order = 3)]
    [Header("Use -1 to pick a random one.", order = 4), Space(-10, order = 5)]
    public int damagePrefabIndex = -1;
}
