using SubLibrary.CyclopsReferencers;
using UnityEngine;

namespace SubLibrary.Monobehaviors;

public class CustomSubVFXConstructing : VFXConstructing, ICyclopsReferencer
{
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
