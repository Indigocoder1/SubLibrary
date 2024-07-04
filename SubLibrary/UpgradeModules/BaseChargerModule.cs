using UnityEngine;

namespace SubLibrary.UpgradeModules;

/// <summary>
/// Handles telling the BaseChargerFunctionality how many of this module are installed, since they can stack.
/// See https://github.com/Indigocoder1/Indigocoder_SubnauticaMods/tree/master/Chameleon/Monobehaviors/UpgradeModules for examples
/// </summary>
/// <typeparam name="T">The <see cref="BaseChargerFunctionality"> for the module</typeparam>
public abstract class BaseChargerModule<T> : MonoBehaviour where T : BaseChargerFunctionality
{
    protected T chargerFunction;

    protected virtual void Awake()
    {
        //This is required since the modules can stack
        chargerFunction = gameObject.EnsureComponent<T>();
        chargerFunction.modulesInstalled++;
    }

    protected virtual void OnDestroy()
    {
        chargerFunction.modulesInstalled--;
    }
}
