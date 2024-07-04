using UnityEngine;

namespace SubLibrary.UpgradeModules;

/// <summary>
/// Handles the charging functionality of a charging module, such as the thermal reactor module.
/// Used in tandem with <see cref="BaseChargerModule{T}"/> to customize module behavior.
/// </summary>
public abstract class BaseChargerFunctionality : MonoBehaviour
{
    public int modulesInstalled;
    protected PowerRelay powerRelay;

    /// <summary>
    /// If greater than 0, calls InvokeRepeating on Awake based on this delay. Otherwise, if 0 or less, UpdateCharge is called every frame on Update.
    /// </summary>
    protected virtual float updateCooldown => -1;

    protected virtual void Awake()
    {
        powerRelay = GetComponentInParent<PowerRelay>();

        if (updateCooldown > 0)
        {
            InvokeRepeating(nameof(UpdateCharge), 1, updateCooldown);
        }
    }

    protected virtual void Update()
    {
        if (updateCooldown <= 0)
        {
            UpdateCharge();
        }
    }

    protected void UpdateCharge()
    {
        if (modulesInstalled <= 0) return;

        powerRelay.AddEnergy(GetCharge() * modulesInstalled, out _);
    }

    /// <summary>
    /// Gets the current charge value
    /// </summary>
    /// <returns>The current value to charge, which will then be multiplied by the number of installed modules and then added to the power relay</returns>
    public abstract float GetCharge();
}
