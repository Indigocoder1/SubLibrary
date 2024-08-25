using Nautilus.Json;
using Newtonsoft.Json;
using System;
using System.Linq;
using UnityEngine;

namespace SubLibrary.SaveData;

public class SubSerializationManager : MonoBehaviour, IProtoEventListener, IProtoTreeEventListener
{
    [HideInInspector] public BaseSubDataClass saveData;

    [SerializeField] private PrefabIdentifier prefabIdentifier;
    [SerializeField, Tooltip("The name of your save data class that inherits from ModuleDataClass. CASE SENSITIVE!")] private string saveDataClassTypeName;

    private void OnValidate()
    {
        if (!this.prefabIdentifier && TryGetComponent(out PrefabIdentifier prefabIdentifier)) this.prefabIdentifier = prefabIdentifier;
    }

    private void Awake()
    {
        saveData = new ModuleDataClass();
    }

    private void OnEnable() => Plugin.SubSaves.OnStartedSaving += OnBeforeSave;
    private void OnDisable() => Plugin.SubSaves.OnStartedSaving -= OnBeforeSave;

    public void OnProtoSerialize(ProtobufSerializer serializer) { }

    public void OnProtoDeserialize(ProtobufSerializer serializer) => OnSaveDataLoaded();

    private void OnSaveDataLoaded()
    {
        var serializedSave = Plugin.SubSaves.saves[prefabIdentifier.Id];

        var saveData = DeserializeSubSaveData(serializedSave);
        this.saveData = saveData;

        foreach (var saveListener in GetComponentsInChildren<ISaveDataListener>(true))
        {
            saveListener.OnSaveDataLoaded(saveData);
        }
    }

    private void OnBeforeSave(object sender, JsonFileEventArgs args)
    {
        foreach (var saveListener in GetComponentsInChildren<ISaveDataListener>(true))
        {
            saveListener.OnBeforeDataSaved(ref saveData);
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
        Type dataClassType = typeof(object);
        try
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            Type type = assemblies.SelectMany(a => a.GetTypes()).Single(t => t != null && t.FullName == saveDataClassTypeName);

            dataClassType = type;
        }
        catch (Exception ex)
        {
            Plugin.Logger.LogError($"Error finding type for data class with name \"{saveDataClassTypeName}\"! Error: {ex.Message}");
        }

        SubSaveData subSaveData = new(dataClassType, serializedData);

        if (!Plugin.SubSaves.saves.ContainsKey(prefabIdentifier.Id))
        {
            Plugin.SubSaves.saves.Add(prefabIdentifier.Id, subSaveData);
        }
        else
        {
            Plugin.SubSaves.saves[prefabIdentifier.Id] = subSaveData;
        }
    }

    public void OnProtoSerializeObjectTree(ProtobufSerializer serializer) { }

    public void OnProtoDeserializeObjectTree(ProtobufSerializer serializer)
    {
        foreach (var saveListener in GetComponentsInChildren<ILateSaveDataListener>(true))
        {
            saveListener.OnLateSaveDataLoaded(saveData);
        }
    }

    private void OnDestroy()
    {
        Plugin.SubSaves.saves.Remove(prefabIdentifier.Id);
    }
}
