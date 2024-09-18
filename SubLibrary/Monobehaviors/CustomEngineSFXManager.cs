using UnityEngine;

namespace SubLibrary.Monobehaviors;

internal class CustomEngineSFXManager : EngineRpmSFXManager
{
    // Called via a patch in EngineRpmSFXManager.Update
    public void OverrideUpdate()
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
    }
}
