using Nautilus.Extensions;
using UnityEngine;

namespace SubLibrary.CyclopsReferencers.RuntimePrefabRetrievers;

internal class SpawnFirePrefabSpawner : MonoBehaviour, ICyclopsReferencer
{
    public void OnCyclopsReferenceFinished(GameObject cyclops)
    {
        PrefabSpawn prefabSpawn = cyclops.GetComponentInChildren<PrefabSpawn>();

        gameObject.AddComponent<PrefabSpawn>().CopyComponent(prefabSpawn);
    }
}
