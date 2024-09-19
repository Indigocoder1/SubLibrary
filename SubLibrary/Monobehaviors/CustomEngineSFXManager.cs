using FMOD;
using Nautilus.Handlers;
using UnityEngine;

namespace SubLibrary.Monobehaviors;

internal class CustomEngineSFXManager : EngineRpmSFXManager
{
    [Tooltip("What volume to be at given a normalized speed value")]
    [SerializeField] private AnimationCurve volumeOverSpeed;

    private Channel loopingChannel;
    private bool foundChannel;

    private void Start()
    {
        foundChannel = CustomSoundHandler.TryGetCustomSoundChannel(engineRpmSFX.GetInstanceID(), out loopingChannel);

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
        if(rpmSpeed > 0f)
        {
            engineRpmSFX.Play();
        }
        else
        {
            engineRpmSFX.Stop();
        }

        if (!foundChannel) return;

        float volume = volumeOverSpeed.Evaluate(rpmSpeed / topClampSpeed);
        loopingChannel.setVolume(volume);

        if (Mathf.Approximately(rpmSpeed, 0f))
        {
            loopingChannel.getPitch(out var currentPitch);
            loopingChannel.setPitch(Mathf.MoveTowards(currentPitch, 0, Time.deltaTime * rampDownSpeed));
        }

        float targetPitch = rpmSpeed / (rpmSpeed * 0.6f) * 2;

        targetPitch = Mathf.Clamp(targetPitch, 0.52f, 1.5f);
        loopingChannel.setPitch(targetPitch);
    }
}
