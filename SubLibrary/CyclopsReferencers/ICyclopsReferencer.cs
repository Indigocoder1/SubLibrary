using UnityEngine;

namespace SubLibrary.CyclopsReferencers;

public interface ICyclopsReferencer
{
    // Code credit:
    // SealSub's ICyclopsReferenceManager https://github.com/32Kallies/SealSub/blob/acafaae6573625c695642439530c93c2280ebf03/SealSubMod/Interfaces/ICyclopsReferencer.cs
    // Contributors:
    // - EldritchCarMaker
    
    /// <summary>
    /// A callback for when <see cref="Handlers.CyclopsReferenceHandler"> retrieves the Cyclops reference.
    /// Can be called by <see cref="Handlers.InterfaceCallerHandler"/> or manually.
    /// </summary>
    /// <param name="cyclops">The reference to the Cyclops</param>
    public void OnCyclopsReferenceFinished(GameObject cyclops);
}
