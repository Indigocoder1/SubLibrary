using Nautilus.Json;
using Newtonsoft.Json;
using System;
using System.Linq;
using UnityEngine;

namespace SubLibrary.SaveData;

public class SubSerializationManager : MonoBehaviour, IProtoEventListener, IProtoTreeEventListener
{
    [HideInInspector] public BaseSubDataClass saveData;

    public PrefabIdentifier prefabIdentifier;
    [Tooltip("The assembly qualified name of your save data class that inherits from BaseSubDataClass. CASE SENSITIVE!")]
    [SerializeField] private string saveDataAssemblyQualifiedName;

    private void OnValidate()
    {
        if (!this.prefabIdentifier && TryGetComponent(out PrefabIdentifier prefabIdentifier)) this.prefabIdentifier = prefabIdentifier;
    }

    private void Awake()
    {
        Plugin.SubSaves.OnStartedSaving += OnBeforeSave;
        
        Plugin.SubSaves.OnFinishedLoading += (a, b) =>
        {
            Plugin.Logger.LogInfo($"Save data finished loading at {Time.realtimeSinceStartup}");
        };
        Initialize();
    }

    private void Initialize()
    {
        if (saveData != null) return;
        
        saveData = new ModuleDataClass();
    }

    public void OnProtoSerialize(ProtobufSerializer serializer) { }

    public void OnProtoDeserialize(ProtobufSerializer serializer) => OnSaveDataLoaded();

    private void OnSaveDataLoaded()
    {
        Initialize();
        
        Plugin.Logger.LogInfo($"Save data loaded called at {Time.realtimeSinceStartup}");
        var serializedSave = Plugin.SubSaves.saves[prefabIdentifier.Id];
        Plugin.Logger.LogInfo($"Serialized save = {serializedSave}");
        
        var saveData = DeserializeSubSaveData(serializedSave);
        Plugin.Logger.LogInfo($"Deserialized save = {saveData}");
        this.saveData = saveData;

        foreach (var saveListener in GetComponentsInChildren<ISaveDataListener>(true))
        {
            Plugin.Logger.LogInfo($"Calling OnSaveDataLoaded on {saveListener}");
            saveListener.OnSaveDataLoaded(saveData);
        }
    }

    private void OnBeforeSave(object sender, JsonFileEventArgs args)
    {
        foreach (var saveListener in GetComponentsInChildren<ISaveDataListener>(true))
        {
            Plugin.Logger.LogInfo($"Calling OnBeforeDataSaved on {saveListener}");
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

        var dataType = TryLoadDataType();
        SubSaveData subSaveData = new(dataType, serializedData);

        if (!Plugin.SubSaves.saves.ContainsKey(prefabIdentifier.Id))
        {
            Plugin.SubSaves.saves.Add(prefabIdentifier.Id, subSaveData);
        }
        else
        {
            Plugin.SubSaves.saves[prefabIdentifier.Id] = subSaveData;
        }
    }

    private Type TryLoadDataType()
    {
        Type dataClassType = typeof(object);
        try
        {
            dataClassType = Type.GetType(saveDataAssemblyQualifiedName);
        }
        catch (Exception ex)
        {
            Plugin.Logger.LogError($"Error finding type for data class with name \"{saveDataAssemblyQualifiedName}\"! Message: {ex.Message}");
        }

        return dataClassType;
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
        Plugin.SubSaves.OnStartedSaving -= OnBeforeSave;
        Plugin.Logger.LogInfo($"On destroy called. Scene loaded = {gameObject.scene.isLoaded}");
        if (!gameObject.scene.isLoaded) return;
        
        Plugin.SubSaves.saves.Remove(prefabIdentifier.Id);
    }
}
