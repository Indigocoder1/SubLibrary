using System.Collections;
using UnityEngine;

namespace SubLibrary.PrefabRetrievers;

public static class CyclopsReferenceHandler
{
    public static GameObject CyclopsReference { get; private set; }
    private static bool loaded;

    /// <summary>
    /// </summary>
    internal static IEnumerator EnsureCyclopsReference()
    {
        if(CyclopsReference)
        {
            yield break;
        }

        loaded = false;

        yield return new WaitUntil(() => LightmappedPrefabs.main);

        LightmappedPrefabs.main.RequestScenePrefab("Cyclops", new LightmappedPrefabs.OnPrefabLoaded(OnPrefabLoaded));

        yield return new WaitUntil(() => loaded);
    }

    private static void OnPrefabLoaded(GameObject gameoObject)
    {
        CyclopsReference = gameoObject;
        loaded = true;
    }
}
