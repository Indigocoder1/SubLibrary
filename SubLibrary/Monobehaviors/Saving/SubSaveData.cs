using System;

namespace SubLibrary.Monobehaviors.Saving;

[Serializable]
internal class SubSaveData
{
    internal Type endTypeToDeserializeTo;
    internal string jsonSerializedData;
}
