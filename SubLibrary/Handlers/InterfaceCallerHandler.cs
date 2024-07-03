using SubLibrary.Interfaces;
using SubLibrary.PrefabRetrievers;
using System.Collections;
using UnityEngine;

namespace SubLibrary.Handlers;

public static class InterfaceCallerHandler
{
    public static IEnumerator InvokeCyclopsReferencers(GameObject prefabRoot)
    {
        yield return CyclopsReferenceManager.EnsureCyclopsReference();

        foreach (var referencer in prefabRoot.GetComponentsInChildren<ICyclopsReferencer>())
        {
            referencer.OnCyclopsReferenceFinished(CyclopsReferenceManager.CyclopsReference);
        }
    }
}
