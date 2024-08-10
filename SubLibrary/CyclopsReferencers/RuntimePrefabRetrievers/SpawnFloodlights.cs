using UnityEngine;

namespace SubLibrary.CyclopsReferencers.RuntimePrefabRetrievers;

internal class SpawnFloodlights : MonoBehaviour, ICyclopsReferencer
{
    public void OnCyclopsReferenceFinished(GameObject cyclops)
    {
        var cyclopsLightParent = cyclops.transform.Find("Floodlights");
        if (!cyclopsLightParent)
        {
            throw new System.Exception("Cyclops floodlights null! Can't create modded sub floodlights");
        }

        foreach (var child in GetComponentsInChildren<FloodlightMarker>(true))
        {
            var light = cyclopsLightParent.Find(child.lightPrefabObjectName);
            if (!light)
            {
                Plugin.Logger.LogError($"Light child {child} has invalid prefab name. Can't find {child.lightPrefabObjectName} child in cyclops floodlights");
            }

            Instantiate(light, child.transform.position, child.transform.rotation, child.transform).gameObject.SetActive(true);
        }
    }
}
