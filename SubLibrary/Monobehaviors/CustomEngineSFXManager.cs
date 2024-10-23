using FMOD;
using Nautilus.Handlers;
using UnityEngine;

namespace SubLibrary.Monobehaviors;

internal class CustomEngineSFXManager : EngineRpmSFXManager
{
    [Tooltip("What volume to be set at a normalized speed value")]
    [SerializeField] private AnimationCurve volumeOverSpeed;
    [Tooltip("What pitch to be set at a normalized speed value")]
    [SerializeField] private AnimationCurve pitchOverSpeed;

    [SerializeField] private bool playLoopSFXWhenNotMoving;

    private CyclopsMotorMode motorMode;
    private Channel loopingChannel;
    private bool foundChannel;

    private void Start()
    {
        engineRpmSFX.Play();
        foundChannel = CustomSoundHandler.TryGetCustomSoundChannel(engineRpmSFX.GetInstanceID(), out loopingChannel);
        engineRpmSFX.Stop(FMOD.Studio.STOP_MODE.IMMEDIATE);

        motorMode = GetComponentInParent<CyclopsMotorMode>();

        if (foundChannel)
        {
            loopingChannel.setPitch(0);
        }
    }

    public void Update()
    {
        HandleRevUp();
        HandleRampUpDown();
        HandleLoopingSFX();

        if (!accelerating) wasAccelerating = false;

        accelerating = false;
    }

    private void HandleRevUp()
    {
        if (engineRevUp == null) return;

        if (accelerating && !wasAccelerating)
        {
            if (rpmSpeed == 0f)
            {
                engineRevUp.Play();
            }
            wasAccelerating = true;
        }
        else if (!accelerating && wasAccelerating)
        {
            engineRevUp.Stop();
            wasAccelerating = false;
        }
    }

    private void HandleRampUpDown()
    {
        if (accelerating)
        {
            rpmSpeed = Mathf.MoveTowards(rpmSpeed, topClampSpeed, Time.deltaTime * rampUpSpeed);
        }
        else
        {
            rpmSpeed = Mathf.MoveTowards(rpmSpeed, 0f, Time.deltaTime * rampDownSpeed);
        }
    }

    private void HandleLoopingSFX()
    {
        if (rpmSpeed > 0f || (playLoopSFXWhenNotMoving && !engineRpmSFX.playing && motorMode.engineOn))
        {
            engineRpmSFX.Play();
        }
        else if ((!playLoopSFXWhenNotMoving && rpmSpeed <= 0) || (playLoopSFXWhenNotMoving && !motorMode.engineOn))
        {
            engineRpmSFX.Stop();
        }

        // The channel has to be updated every frame for some reason
        if (!CustomSoundHandler.TryGetCustomSoundChannel(engineRpmSFX.GetInstanceID(), out loopingChannel)) return;

        float normalizedSpeed = rpmSpeed / topClampSpeed;
        float volume = volumeOverSpeed.Evaluate(normalizedSpeed);
        loopingChannel.setVolume(volume);

        if (Mathf.Approximately(rpmSpeed, 0f))
        {
            loopingChannel.getPitch(out var currentPitch);
            loopingChannel.setPitch(Mathf.MoveTowards(currentPitch, 0, Time.deltaTime * rampDownSpeed));
        }

        float targetPitch = pitchOverSpeed.Evaluate(normalizedSpeed);
        if (targetPitch < 0.52f || targetPitch > 1.5f)
        {
            Plugin.Logger.LogWarning($"Pitch on {gameObject} is exceeding the recommended values of min 0.52, max 1.5 at {targetPitch}");
        }

        loopingChannel.setPitch(targetPitch);
    }
}
