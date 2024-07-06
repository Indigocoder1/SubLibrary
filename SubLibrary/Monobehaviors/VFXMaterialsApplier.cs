using SubLibrary.CyclopsReferencers;
using UnityEngine;

namespace SubLibrary.Monobehaviors;

public class VFXMaterialsApplier : MonoBehaviour, ICyclopsReferencer
{
    [SerializeField] private VFXConstructing vfxConstructing;

    private void OnValidate()
    {
        if (!vfxConstructing && TryGetComponent(out VFXConstructing constructing)) vfxConstructing = constructing;
    }

    public void OnCyclopsReferenceFinished(GameObject cyclops)
    {
        var cyclopsConstructing = cyclops.GetComponent<VFXConstructing>();
        vfxConstructing.ghostMaterial = cyclopsConstructing.ghostMaterial;
        vfxConstructing.alphaTexture = cyclopsConstructing.alphaTexture;
        vfxConstructing.alphaDetailTexture = cyclopsConstructing.alphaDetailTexture;
        vfxConstructing.transparentShaders = cyclopsConstructing.transparentShaders;
        vfxConstructing.surfaceSplashFX = cyclopsConstructing.surfaceSplashFX;
    }
}
