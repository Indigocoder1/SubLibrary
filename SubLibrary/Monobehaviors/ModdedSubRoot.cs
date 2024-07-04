using SubLibrary.Interfaces;
using SubLibrary.Monobehaviors.Saving;
using System;
using UnityEngine;

namespace SubLibrary.Monobehaviors;

public abstract class ModdedSubRoot : SubRoot, ISaveDataListener
{
    protected BaseModdedSubSaveData saveData;

    [SerializeField] private Type saveDataClassType;

    private void Awake()
    {
        saveData = new();
    }

    public void OnSaveDataLoaded(BaseModdedSubSaveData saveData)
    {
        this.saveData = saveData;
    }

    public void OnBeforeDataSaved(BaseModdedSubSaveData saveData)
    {
        
    }
}
