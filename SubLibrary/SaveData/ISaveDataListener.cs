using SubLibrary.SaveData;

namespace SubLibrary.Interfaces;

public interface ISaveDataListener
{
    public void OnSaveDataLoaded(BaseSubDataClass saveData);
    public void OnBeforeDataSaved();
}
