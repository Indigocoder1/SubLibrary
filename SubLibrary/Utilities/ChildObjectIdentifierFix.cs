using SubLibrary.Monobehaviors;

namespace SubLibrary.Utilities;

internal class ChildObjectIdentifierFix : PrefabModifier
{
    // Code credit:
    // SealSub's ChildObjectIdentifierFix https://github.com/32Kallies/SealSub/blob/main/SealSubMod/MonoBehaviours/Prefab/ChildObjectIdentifierFix.cs
    // Contributors:
    // - Kallie23
    // - EldritchCarMaker
    
    public string classID;
    public ChildObjectIdentifier childObjectIdentifier;

    public override void OnAsyncPrefabTasksCompleted()
    {
        childObjectIdentifier.classId = classID;
    }

    private void OnValidate()
    {
        if (!childObjectIdentifier)
        {
            childObjectIdentifier = GetComponent<ChildObjectIdentifier>();
        }

        if (string.IsNullOrEmpty(classID))
        {
            classID = System.Guid.NewGuid().ToString();
        }
    }
}
