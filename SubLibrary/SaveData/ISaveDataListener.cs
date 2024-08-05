namespace SubLibrary.SaveData;

public interface ISaveDataListener
{
    /// <summary>
    /// Called after save data is loaded from the SaveDataCache
    /// </summary>
    /// <param name="saveData">The save data</param>
    public void OnSaveDataLoaded(BaseSubDataClass saveData);

    /// <summary>
    /// Called before save data is put into the SaveDataCache
    /// </summary>
    /// <param name="saveData">The save data</param>
    public void OnBeforeDataSaved(ref BaseSubDataClass saveData);
}
