using SubLibrary.Audio;
using SubLibrary.Materials;
using SubLibrary.Monobehaviors;
using SubLibrary.SaveData;
using UnityEngine;
using UWE;

namespace SubLibrary.Utilities;

[ExecuteAlways]
internal class AddSubScripts : MonoBehaviour
{
    private bool destroy;

    private void OnValidate()
    {
        var rb = gameObject.EnsureComponent<Rigidbody>();
        var subRoot = gameObject.EnsureComponent<SubRoot>();
        var idenitifer = gameObject.EnsureComponent<PrefabIdentifier>();
        gameObject.EnsureComponent<TechTag>();
        var lwe = gameObject.EnsureComponent<LargeWorldEntity>();
        var exteriorApplier = gameObject.AddComponent<SkyApplier>();
        var interiorApplier = gameObject.AddComponent<SkyApplier>();
        var glassApplier = gameObject.AddComponent<SkyApplier>();
        var worldForces = gameObject.EnsureComponent<WorldForces>();
        var lightingController = gameObject.EnsureComponent<LightingController>();
        var lod = gameObject.EnsureComponent<BehaviourLOD>();
        var mixin = gameObject.EnsureComponent<LiveMixin>();
        gameObject.EnsureComponent<Stabilizer>();
        var relay = gameObject.EnsureComponent<PowerRelay>();
        var motorMode = gameObject.EnsureComponent<CyclopsMotorMode>();
        var subControl = gameObject.EnsureComponent<SubControl>();
        var freezeWhenFar = gameObject.EnsureComponent<FreezeRigidbodyWhenFar>();
        gameObject.EnsureComponent<PingInstance>();
        var notificationManager = gameObject.EnsureComponent<VoiceNotificationManager>();
        var crushDamage = gameObject.EnsureComponent<CrushDamage>();
        var conditionRules = gameObject.EnsureComponent<ConditionRules>();
        var depthAlarms = gameObject.EnsureComponent<DepthAlarms>();
        var noiseManager = gameObject.EnsureComponent<CyclopsNoiseManager>();
        gameObject.EnsureComponent<ToggleLights>();
        var vfxConstructing = gameObject.EnsureComponent<CustomSubVFXConstructing>();
        gameObject.EnsureComponent<DealDamageOnImpact>();
        gameObject.EnsureComponent<OxygenManager>();
        var serializationManager = gameObject.EnsureComponent<SubSerializationManager>();
        gameObject.EnsureComponent<SubAudioLoader>();
        var skyManager = gameObject.EnsureComponent<SubSkyManager>();

        gameObject.AddComponent<RemoveBangsFromSmallFish>();
        gameObject.AddComponent<OnTakeDamageRelay>();

        subRoot.rb = rb;
        subRoot.lightControl = lightingController;
        subRoot.noiseManager = noiseManager;
        subRoot.powerRelay = relay;
        subRoot.LOD = lod;

        lwe.cellLevel = LargeWorldEntity.CellLevel.Global;

        worldForces.useRigidbody = rb;

        motorMode.subRoot = subRoot;
        motorMode.subController = subControl;

        subControl.powerRelay = relay;
        subControl.LOD = lod;

        notificationManager.subRoot = subRoot;
        crushDamage.liveMixin = mixin;

        depthAlarms.crushDamage = crushDamage;
        depthAlarms.conditionRules = conditionRules;

        noiseManager.subRoot = subRoot;
        noiseManager.cyclopsMotorMode = motorMode;
        noiseManager.subControl = subControl;

        if (vfxConstructing.disableBehaviours == null)
        {
            vfxConstructing.disableBehaviours = new();
        }

        vfxConstructing.disableBehaviours.Add(subRoot);
        vfxConstructing.disableBehaviours.Add(freezeWhenFar);

        serializationManager.prefabIdentifier = idenitifer;

        skyManager.exteriorSkyApplier = exteriorApplier;
        skyManager.interiorSkyApplier = interiorApplier;
        skyManager.windowSkyApplier = glassApplier;
        skyManager.lightingController = lightingController;

        destroy = true;
    }

    private void Update()
    {
        if (destroy)
        {
            DestroyImmediate(this);
        }
    }
}
