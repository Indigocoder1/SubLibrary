using UnityEngine;

namespace SubLibrary.RuntimeSpawners;

public interface ICyclopsReferencer
{
    /// <summary>
    /// A callback for when <see cref="Handlers.CyclopsReferenceHandler"> retrieves the Cyclops reference.
    /// Can be called by <see cref="Handlers.InterfaceCallerHandler"/> or manually.
    /// </summary>
    /// <param name="cyclops">The reference to the Cyclops</param>
    public void OnCyclopsReferenceFinished(GameObject cyclops);
}
