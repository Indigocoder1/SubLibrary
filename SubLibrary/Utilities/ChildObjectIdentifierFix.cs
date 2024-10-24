using SubLibrary.Monobehaviors;

namespace SubLibrary.Utilities;

internal class ChildObjectIdentifierFix : PrefabModifier
{
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
