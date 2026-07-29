using UnityEngine;

namespace SubLibrary.UpgradeModules;

public abstract class BaseDepthModule : MonoBehaviour, IOnModulesChanged
{
    // Code credit:
    // SealSub's DepthModule https://github.com/32Kallies/SealSub/blob/main/SealSubMod/MonoBehaviours/UpgradeModules/DepthModule.cs
    // Contributors:
    // - EldritchCarMaker
    
    public abstract float Depth { get; }
    private CrushDamage _damage;

    protected CrushDamage CrushDamage
    {
        get
        {
            if (!_damage) _damage = gameObject.GetComponentInParent<CrushDamage>(true);

            return _damage;
        }
    }

    public void OnChange(TechType techType, bool added)
    {
        float depth = Mathf.Max(Depth, CrushDamage.extraCrushDepth);
        CrushDamage.SetExtraCrushDepth(depth);
    }

    private void OnDisable()
    {
        _damage.SetExtraCrushDepth(0);
    }
}
