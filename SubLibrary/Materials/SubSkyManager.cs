using mset;
using SubLibrary.CyclopsReferencers;
using SubLibrary.Materials.Tags;
using SubLibrary.Monobehaviors;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SubLibrary.Materials;

internal class SubSkyManager : PrefabModifier, ICyclopsReferencer
{
    public SkyApplier exteriorSkyApplier;
    public SkyApplier interiorSkyApplier;
    public SkyApplier windowSkyApplier;
    [SerializeField] private float[] lightBrightnessMultipliers = new[] { 1, 0.5f, 0f };

    public LightingController lightingController;

    private List<Renderer> _interiorRenderers = new();
    private List<Renderer> _exteriorRenderers = new();
    private List<Renderer> _windowRenderers = new();

    public override void OnAsyncPrefabTasksCompleted()
    {
        var allRenderers = gameObject.GetComponentsInChildren<Renderer>(true);
        foreach (var renderer in allRenderers)
        {
            List<Renderer> list;
            if (renderer.gameObject.TryGetComponent(out SubWindowTag _))
            {
                list = _windowRenderers;
            }
            else if (renderer.gameObject.TryGetComponent(out SubExteriorTag _))
            {
                list = _exteriorRenderers;
            }
            else
            {
                list = _interiorRenderers;
            }

            list.Add(renderer);
        }

        exteriorSkyApplier.renderers = _exteriorRenderers.ToArray();
        interiorSkyApplier.renderers = _interiorRenderers.ToArray();
        windowSkyApplier.renderers = _windowRenderers.ToArray();
    }

    public void OnCyclopsReferenceFinished(GameObject cyclops)
    {
        var skyBaseGlass = Instantiate(cyclops.transform.Find("SkyBaseGlass"), transform).GetComponent<Sky>();
        var skyBaseInterior = Instantiate(cyclops.transform.Find("SkyBaseInterior"), transform).GetComponent<Sky>();
        lightingController.skies[0].sky = skyBaseGlass;
        lightingController.skies[1].sky = skyBaseInterior;

        var lights = GetComponentsInChildren<Light>(true)
            .Where(l => l.GetComponent<ExcludeFromLightingController>() == null)
            .ToArray();

        lightingController.lights = new MultiStatesLight[lights.Length];
        for (int i = 0; i < lights.Length; i++)
        {
            var intensity = lights[i].intensity;
            lightingController.lights[i] = new MultiStatesLight()
            {
                light = lights[i],
                intensities = new[]
                {
                    intensity * lightBrightnessMultipliers[0],
                    intensity * lightBrightnessMultipliers[1],
                    intensity * lightBrightnessMultipliers[2]
                }
            };
        }
    }
}
