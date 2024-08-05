using SubLibrary.CyclopsReferencers;
using System.Collections;
using UnityEngine;

namespace SubLibrary.Handlers;

public static class InterfaceCallerHandler
{
    /// <summary>
    /// Calls <see cref="ICyclopsReferencer.OnCyclopsReferenceFinished(GameObject)"/> on every cyclops referencer in the children of prefabRoot
    /// </summary>
    /// <param name="prefabRoot">The root of the prefab</param>
    /// <returns></returns>
    public static IEnumerator InvokeCyclopsReferencers(GameObject prefabRoot)
    {
        yield return CyclopsReferenceHandler.EnsureCyclopsReference();

        foreach (var referencer in prefabRoot.GetComponentsInChildren<ICyclopsReferencer>())
        {
            referencer.OnCyclopsReferenceFinished(CyclopsReferenceHandler.CyclopsReference);
        }
    }
}
