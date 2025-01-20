using System.Linq;
using UnityEngine;

namespace SubLibrary.Monobehaviors;

internal class OnTakeDamageRelay : MonoBehaviour, IOnTakeDamage
{
    [SerializeField] private LiveMixin mixin;
    [SerializeField] private CyclopsExternalDamageManager damageManager;
    private IOnTakeDamage[] listeners;

    private void OnValidate()
    {
        if (!mixin) TryGetComponent(out mixin);
    }

    private void Start()
    {
        listeners = GetComponentsInChildren<IOnTakeDamage>(true).Where(l =>
        {
            return l != (IOnTakeDamage)this && !mixin.damageReceivers.Contains(l) && !damageManager.damageRecievers.Contains(l);
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
