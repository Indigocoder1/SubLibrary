namespace SubLibrary.SaveData;

public interface ILateSaveDataListener
{
    /// <summary>
    /// Called after <see cref="IProtoTreeEventListener.OnProtoDeserializeObjectTree(ProtobufSerializer)"/> (Serialized prefabs will be loaded in)
    /// </summary>
    /// <param name="saveData">The save data</param>
    public void OnLateSaveDataLoaded(BaseSubDataClass saveData);
}
