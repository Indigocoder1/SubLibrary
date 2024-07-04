using Newtonsoft.Json;
using SubLibrary.Interfaces;
using SubLibrary.Monobehaviors.Saving;
using System;
using UnityEngine;

namespace SubLibrary.Monobehaviors;

public abstract class ModdedSubRoot : SubRoot, ISaveDataListener
{
    public BaseModdedSubSaveData saveData;

    [SerializeField] private PrefabIdentifier prefabIdentifier;
    [SerializeField] private Type saveDataClassType;

    private void OnValidate()
    {
        if(prefabIdentifier == null && TryGetComponent(out PrefabIdentifier identifier)) prefabIdentifier = identifier;
    }

    private void Awake()
    {
        base.Awake();
        //Create new save data for this sub
        saveData = new();
    }

    public void OnSaveDataLoaded(BaseModdedSubSaveData saveData)
    {
        //Only called if save data actually exists for this sub
        this.saveData = saveData;
    }

    public void OnBeforeDataSaved(BaseModdedSubSaveData saveData)
    {
        string serializedData = JsonConvert.SerializeObject(saveData);
        SubSaveData subSaveData = new(saveDataClassType, serializedData);

        if(!SubSerializationManager.SubSaves.ContainsKey(prefabIdentifier.Id))
        {
            SubSerializationManager.SubSaves.Add(prefabIdentifier.Id, subSaveData);
        }
        else
        {
            SubSerializationManager.SubSaves[prefabIdentifier.Id] = subSaveData;
        }
    }
}
