using SubLibrary.Interfaces;
using UnityEngine;

namespace SubLibrary.Monobehaviors;

public class VFXMaterialApplier : MonoBehaviour, ICyclopsReferencer
{
    [SerializeField] private float waterLevelYOffset = 3;
    [SerializeField] private VFXConstructing vfxConstructing;
    [SerializeField] private CyclopsExternalDamageManager damageManager;

    public void OnCyclopsReferenceFinished(GameObject cyclops)
    {
        var cyclopsConstructing = cyclops.GetComponent<VFXConstructing>();
        vfxConstructing.ghostMaterial = cyclopsConstructing.ghostMaterial;
        vfxConstructing.alphaTexture = cyclopsConstructing.alphaTexture;
        vfxConstructing.alphaDetailTexture = cyclopsConstructing.alphaDetailTexture;
        vfxConstructing.transparentShaders = cyclopsConstructing.transparentShaders;
        vfxConstructing.surfaceSplashFX = cyclopsConstructing.surfaceSplashFX;

        damageManager.fxPrefabs = cyclops.GetComponentInChildren<CyclopsExternalDamageManager>(true).fxPrefabs;
    }
}
