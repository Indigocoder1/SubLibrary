using System.Linq;
using UnityEngine;

namespace SubLibrary.Monobehaviors;

internal class OnTakeDamageRelay : MonoBehaviour, IOnTakeDamage
{
    private IOnTakeDamage[] listeners;
    private LiveMixin mixin;

    private void Start()
    {
        mixin = GetComponent<LiveMixin>();
        listeners = GetComponentsInChildren<IOnTakeDamage>(true).Where(l =>
        {
            return l != (IOnTakeDamage)this && !mixin.damageReceivers.Contains(l);
        }).ToArray();
    }

    public void OnTakeDamage(DamageInfo damageInfo)
    {
        foreach (var item in listeners)
        {
            item.OnTakeDamage(damageInfo);
        }
    }
}
