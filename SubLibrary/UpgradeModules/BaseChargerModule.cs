using UnityEngine;

namespace SubLibrary.UpgradeModules;

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
