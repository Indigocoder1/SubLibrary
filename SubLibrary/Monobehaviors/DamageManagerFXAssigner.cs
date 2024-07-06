using Discord;
using SubLibrary.CyclopsReferencers;
using UnityEngine;

namespace SubLibrary.Monobehaviors;

internal class DamageManagerFXAssigner : MonoBehaviour, ICyclopsReferencer
{
    [SerializeField] private CyclopsExternalDamageManager damageManager;

    private void OnValidate()
    {
        if(!damageManager && TryGetComponent(out CyclopsExternalDamageManager manager)) damageManager = manager;
    }

    public void OnCyclopsReferenceFinished(GameObject cyclops)
    {
        damageManager.fxPrefabs = cyclops.GetComponentInChildren<CyclopsExternalDamageManager>(true).fxPrefabs;
    }
}
