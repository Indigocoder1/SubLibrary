using Nautilus.Utility;
using SubLibrary.Handlers;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace SubLibrary.Materials;

public class MaterialSetter : MonoBehaviour
{
    [SerializeField] private MaterialMode mode;
    [SerializeField] private bool runAtStart = true;

    [Header("For single renderer mode")]
    [SerializeField] private Renderer renderer;
    [SerializeField] private int[] materialIndices = new[] { 0 };

    [Header("The material to apply")]
    [SerializeField] private MaterialType materialType;

    private static Material glassMaterial;
    private static Material exteriorGlassMaterial;
    private static Material shinyGlassMaterial;
    private static Material interiorWindowGlassMaterial;
    private static Material holographicUIMaterial;

    private void OnValidate()
    {
        if (!renderer) renderer = GetComponent<Renderer>();
    }

    private void Start()
    {
        if (!runAtStart) return;

        AssignMaterials();
    }

    public void AssignMaterials()
    {
        switch (mode)
        {
            case MaterialMode.SingleRenderer:
                if (!renderer) throw new System.Exception($"Renderer is null on material setter {name}");

                var mats = renderer.materials;
                foreach (var index in materialIndices)
                {
                    mats[index] = GetMaterial(materialType);
                }
                renderer.materials = mats;
                break;

            case MaterialMode.AllChildRenderers:
                foreach (var childRend in GetComponentsInChildren<Renderer>(true))
                {
                    var childRendMats = childRend.materials;
                    for (int i = 0; i < childRendMats.Length; i++)
                    {
                        childRendMats[i] = GetMaterial(materialType);
                    }
                    childRend.materials = childRendMats;
                }
                break;

            case MaterialMode.AllChildGraphics:
                foreach (var graphic in GetComponentsInChildren<Graphic>(true))
                {
                    graphic.material = GetMaterial(materialType);
                }
                break;
        }
    }

    public static Material GetMaterial(MaterialType type)
    {
        Material mat = type switch
        {
            MaterialType.WaterBarrier => MaterialUtils.AirWaterBarrierMaterial,
            MaterialType.ForceField => MaterialUtils.ForceFieldMaterial,
            MaterialType.StasisField => MaterialUtils.StasisFieldMaterial,
            MaterialType.Glass => glassMaterial,
            MaterialType.ExteriorGlass => exteriorGlassMaterial,
            MaterialType.ShinyGlass => shinyGlassMaterial,
            MaterialType.InteriorWindowGlass => interiorWindowGlassMaterial,
            MaterialType.HolographicUI => holographicUIMaterial,
            _ => null
        };

        return new Material(mat);
    }

    public static IEnumerator LoadMaterialsAsync()
    {
        var seamothTask = CraftData.GetPrefabForTechTypeAsync(TechType.Seamoth);

        yield return seamothTask;

        var seamothGlassMaterial = seamothTask.GetResult()
            .transform.Find("Model/Submersible_SeaMoth/Submersible_seaMoth_geo/Submersible_SeaMoth_glass_geo")
            .GetComponent<Renderer>().material;

        glassMaterial = new Material(seamothGlassMaterial);

        exteriorGlassMaterial = new Material(seamothGlassMaterial);
        exteriorGlassMaterial.SetFloat("_SpecInt", 100);
        exteriorGlassMaterial.SetFloat("_Shininess", 6.3f);
        exteriorGlassMaterial.SetFloat("_Fresnel", 0.85f);
        exteriorGlassMaterial.SetColor("_Color", new Color(0.33f, 0.58f, 0.71f, 0.1f));
        exteriorGlassMaterial.SetColor("_SpecColor", new Color(0.5f, 0.76f, 1f, 1f));

        shinyGlassMaterial = new Material(seamothGlassMaterial);
        shinyGlassMaterial.SetColor("_Color", new Color(1, 1, 1, 0.2f));
        shinyGlassMaterial.SetFloat("_SpecInt", 3);
        shinyGlassMaterial.SetFloat("_Shininess", 8);
        shinyGlassMaterial.SetFloat("_Fresnel", 0.78f);

        interiorWindowGlassMaterial = new Material(seamothGlassMaterial);
        interiorWindowGlassMaterial.SetColor("_Color", new Color(0.67f, 0.71f, 0.76f, 0.56f));
        interiorWindowGlassMaterial.SetFloat("_SpecInt", 2);
        interiorWindowGlassMaterial.SetFloat("_Shininess", 6f);
        interiorWindowGlassMaterial.SetFloat("_Fresnel", 0.88f);

        yield return CyclopsReferenceHandler.EnsureCyclopsReference();

        var holoMat = CyclopsReferenceHandler.CyclopsReference.transform.Find("HelmHUD/HelmHUDVisuals/Canvas_LeftHUD/EngineOnUI/EngineOff_Button")
            .GetComponent<Image>().material;

        holographicUIMaterial = new Material(holoMat);
    }

    public enum MaterialType
    {
        WaterBarrier,
        ForceField,
        StasisField,
        Glass,
        ExteriorGlass,
        ShinyGlass,
        InteriorWindowGlass,
        HolographicUI
    }

    public enum MaterialMode
    {
        SingleRenderer,
        AllChildRenderers,
        AllChildGraphics
    }
}
