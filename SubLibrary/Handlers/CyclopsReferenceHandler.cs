﻿using System.Collections;
using UnityEngine;

namespace SubLibrary.Handlers;

public static class CyclopsReferenceHandler
{
    public static GameObject CyclopsReference { get; private set; }
    private static bool loaded;

    /// <summary>
    /// Waits for <see cref="LightmappedPrefabs.main"/> to be initialized, then caches the Cyclops prefab.
    /// </summary>
    public static IEnumerator EnsureCyclopsReference()
    {
        if (CyclopsReference)
        {
            yield break;
        }

        loaded = false;

        yield return new WaitUntil(() => LightmappedPrefabs.main);

        LightmappedPrefabs.main.RequestScenePrefab("Cyclops", new LightmappedPrefabs.OnPrefabLoaded(OnPrefabLoaded));

        yield return new WaitUntil(() => loaded);
    }

    private static void OnPrefabLoaded(GameObject gameObject)
    {
        CyclopsReference = gameObject;
        loaded = true;
    }
}
