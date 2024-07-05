using System.Collections.Generic;
using UnityEngine;

namespace SubLibrary.CyclopsReferencers.RuntimePrefabRetrievers;

internal class SpawnCyclopsDamagePoints : MonoBehaviour, ICyclopsReferencer
{
    [SerializeField] private CyclopsExternalDamageManager damageManager;

    public void OnCyclopsReferenceFinished(GameObject cyclops)
    {
        var cyclopsManager = cyclops.GetComponentInChildren<CyclopsExternalDamageManager>();

        var points = new List<CyclopsDamagePoint>();

        foreach (var slot in GetComponentsInChildren<DamagePointSlot>())
        {
            if (slot.damagePrefabIndex <= -1) slot.damagePrefabIndex = Random.Range(0, cyclopsManager.damagePoints.Length);

            var prefab = cyclopsManager.damagePoints[slot.damagePrefabIndex].gameObject;
            var copy = Instantiate(prefab, slot.transform.position, slot.transform.rotation, slot.transform);

            points.Add(copy.GetComponent<CyclopsDamagePoint>());
        }

        damageManager.damagePoints = points.ToArray();
    }
}
