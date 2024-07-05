using SubLibrary.CyclopsReferencers;
using System.Collections;
using UnityEngine;

namespace SubLibrary.Handlers;

public static class InterfaceCallerHandler
{
    public static IEnumerator InvokeCyclopsReferencers(GameObject prefabRoot)
    {
        yield return CyclopsReferenceHandler.EnsureCyclopsReference();

        foreach (var referencer in prefabRoot.GetComponentsInChildren<ICyclopsReferencer>())
        {
            referencer.OnCyclopsReferenceFinished(CyclopsReferenceHandler.CyclopsReference);
        }
    }
}
