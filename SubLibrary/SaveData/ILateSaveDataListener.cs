namespace SubLibrary.SaveData;

public interface ILateSaveDataListener
{
    public void OnLateSaveDataLoaded(BaseSubDataClass saveData);
}
