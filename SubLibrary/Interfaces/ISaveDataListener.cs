using SubLibrary.Monobehaviors.Saving;

namespace SubLibrary.Interfaces;

public interface ISaveDataListener
{
    public void OnSaveDataLoaded(BaseModdedSubSaveData saveData);
    public void OnBeforeDataSaved(BaseModdedSubSaveData saveData);
}
