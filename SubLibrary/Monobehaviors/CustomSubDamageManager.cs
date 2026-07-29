using SubLibrary.CyclopsReferencers;
using UnityEngine;

namespace SubLibrary.Monobehaviors;

internal class CustomSubDamageManager : CyclopsExternalDamageManager, ICyclopsReferencer
{
    // Code credit:
    // SealSub's DamageManagerPrefabSetter https://github.com/32Kallies/SealSub/blob/main/SealSubMod/MonoBehaviours/Prefab/DamageManagerPrefabSetter.cs
    // (Modified)
    // Contributors:
    // - EldritchCarMaker
    
    public void OnCyclopsReferenceFinished(GameObject cyclops)
    {
        fxPrefabs = cyclops.GetComponentInChildren<CyclopsExternalDamageManager>(true).fxPrefabs;
    }
}
