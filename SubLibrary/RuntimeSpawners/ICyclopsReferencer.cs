using UnityEngine;

namespace SubLibrary.Interfaces;

public interface ICyclopsReferencer
{
    /// <summary>
    /// A callback for when <see cref="PrefabRetrievers.CyclopsReferenceHandler"> retrieves the Cyclops reference.
    /// Can be called by <see cref="Handlers.InterfaceCallerHandler"/> or manually.
    /// </summary>
    /// <param name="cyclops">The reference to the Cyclops</param>
    public void OnCyclopsReferenceFinished(GameObject cyclops);
}
