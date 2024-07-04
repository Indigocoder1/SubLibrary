namespace SubLibrary.SaveData;

public interface ISaveDataListener
{
    public void OnSaveDataLoaded(BaseSubDataClass saveData);
    public void OnBeforeDataSaved();
}
