using Newtonsoft.Json;
using System;

namespace SubLibrary.SaveData;

[Serializable]
internal struct SubSaveData
{
    [JsonProperty] internal Type endTypeToDeserializeTo;
    [JsonProperty] internal string jsonSerializedData;

    public SubSaveData(Type endTypeToDeserializeTo, string jsonSerializedData)
    {
        this.endTypeToDeserializeTo = endTypeToDeserializeTo;
        this.jsonSerializedData = jsonSerializedData;
    }
}
