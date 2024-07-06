using SubLibrary.CyclopsReferencers;
using UnityEngine;

namespace SubLibrary.Monobehaviors;

internal class CustomSubDamageManager : CyclopsExternalDamageManager, ICyclopsReferencer
{
    public void OnCyclopsReferenceFinished(GameObject cyclops)
    {
        fxPrefabs = cyclops.GetComponentInChildren<CyclopsExternalDamageManager>(true).fxPrefabs;
    }
}
