using SubLibrary.CyclopsReferencers;
using UnityEngine;
using UnityEngine.Events;

namespace SubLibrary.Monobehaviors;

public class CustomSubVFXConstructing : VFXConstructing, ICyclopsReferencer
{
    public UnityEvent onConstructionStarted;
    public UnityEvent onConstructionFinished;

    public void OnCyclopsReferenceFinished(GameObject cyclops)
    {
        var cyclopsConstructing = cyclops.GetComponent<VFXConstructing>();
        ghostMaterial = cyclopsConstructing.ghostMaterial;
        alphaTexture = cyclopsConstructing.alphaTexture;
        alphaDetailTexture = cyclopsConstructing.alphaDetailTexture;
        transparentShaders = cyclopsConstructing.transparentShaders;
        surfaceSplashFX = cyclopsConstructing.surfaceSplashFX;
    }
}
