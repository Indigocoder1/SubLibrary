using UnityEngine;

namespace SubLibrary.Monobehaviors;

public abstract class PrefabModifier : MonoBehaviour
{
    public virtual void OnAsyncPrefabTasksCompleted() { }
    public virtual void OnLateMaterialOperation() { }
}
