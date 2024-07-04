using Newtonsoft.Json;
using SubLibrary.Interfaces;
using System.Collections.Generic;
using UnityEngine;

namespace SubLibrary.Monobehaviors.Saving;

internal class SubSerializationManager : MonoBehaviour, IProtoEventListener
{
    [SerializeField] private PrefabIdentifier prefabIdentifier;

    public static Dictionary<string, SubSaveData> SubSaves = new();

    private void OnValidate()
    {
        if (!this.prefabIdentifier && TryGetComponent(out PrefabIdentifier prefabIdentifier)) this.prefabIdentifier = prefabIdentifier;
    }

    public void OnProtoDeserialize(ProtobufSerializer serializer)
    {
        BaseModdedSubSaveData saveData = DeserializeSubSaveData(SubSaves[prefabIdentifier.Id]);

        foreach (var saveListener in GetComponentsInChildren<ISaveDataListener>(true))
        {
            saveListener.OnSaveDataLoaded(saveData);
        }
    }

    public void OnProtoSerialize(ProtobufSerializer serializer)
    {
        BaseModdedSubSaveData saveData = DeserializeSubSaveData(SubSaves[prefabIdentifier.Id]);

        foreach (var saveListener in GetComponentsInChildren<ISaveDataListener>(true))
        {
            saveListener.OnBeforeDataSaved(saveData);
        }
    }

    internal BaseModdedSubSaveData DeserializeSubSaveData(SubSaveData saveData)
    {
        return (BaseModdedSubSaveData)JsonConvert.DeserializeObject(saveData.jsonSerializedData, saveData.endTypeToDeserializeTo);
    }
}
