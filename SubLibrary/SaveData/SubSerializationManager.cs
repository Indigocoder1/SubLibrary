using Newtonsoft.Json;
using SubLibrary.Interfaces;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SubLibrary.SaveData;

internal class SubSerializationManager : MonoBehaviour, IProtoEventListener
{
    public static Dictionary<string, SubSaveData> SubSaves = new();

    [HideInInspector] public BaseSubDataClass saveData;

    [SerializeField] private PrefabIdentifier prefabIdentifier;
    [SerializeField] private Type saveDataClassType;

    private void OnValidate()
    {
        if (!this.prefabIdentifier && TryGetComponent(out PrefabIdentifier prefabIdentifier)) this.prefabIdentifier = prefabIdentifier;
    }

    private void Awake()
    {
        saveData = new ModuleDataClass();
    }

    public void OnProtoDeserialize(ProtobufSerializer serializer)
    {
        var serializedSave = SubSaves[prefabIdentifier.Id];

        var saveData = DeserializeSubSaveData(serializedSave);
        this.saveData = saveData;

        foreach (var saveListener in GetComponentsInChildren<ISaveDataListener>(true))
        {
            saveListener.OnSaveDataLoaded(saveData);
        }
    }

    public void OnProtoSerialize(ProtobufSerializer serializer)
    {
        foreach (var saveListener in GetComponentsInChildren<ISaveDataListener>(true))
        {
            saveListener.OnBeforeDataSaved();
        }

        UpdateDictionarySaveData();
    }

    internal ModuleDataClass DeserializeSubSaveData(SubSaveData saveData)
    {
        return JsonConvert.DeserializeObject(saveData.jsonSerializedData, saveData.endTypeToDeserializeTo) as ModuleDataClass;
    }

    private void UpdateDictionarySaveData()
    {
        string serializedData = JsonConvert.SerializeObject(saveData);
        SubSaveData subSaveData = new(saveDataClassType, serializedData);

        if (!SubSaves.ContainsKey(prefabIdentifier.Id))
        {
            SubSaves.Add(prefabIdentifier.Id, subSaveData);
        }
        else
        {
            SubSaves[prefabIdentifier.Id] = subSaveData;
        }
    }
}
