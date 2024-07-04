using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SubLibrary.UpgradeModules.BaseClasses;

public abstract class BaseDepthModule : MonoBehaviour, IOnModulesChanged
{
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
