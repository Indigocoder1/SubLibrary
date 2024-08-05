using UnityEngine;

namespace SubLibrary.Monobehaviors;

public abstract class PrefabModifier : MonoBehaviour
{
    /// <summary>
    /// Must be called when setting up your prefab after all async tasks are completed
    /// </summary>
    public virtual void OnAsyncPrefabTasksCompleted() { }

    /// <summary>
    /// Must be called when setting up your prefab after all material applications are finished
    /// </summary>
    public virtual void OnLateMaterialOperation() { }
}
