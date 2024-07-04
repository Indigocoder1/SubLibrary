using System;

namespace SubLibrary.Monobehaviors.Saving;

[Serializable]
internal struct SubSaveData
{
    internal Type endTypeToDeserializeTo;
    internal string jsonSerializedData;

    public SubSaveData(Type endTypeToDeserializeTo, string jsonSerializedData)
    {
        this.endTypeToDeserializeTo = endTypeToDeserializeTo;
        this.jsonSerializedData = jsonSerializedData;
    }
}
