using Nautilus.Json;
using Nautilus.Json.Attributes;
using System.Collections.Generic;

namespace SubLibrary.SaveData;

[FileName("SubLibrarySaveData")]
internal class SubLibrarySaveDataCache : SaveDataCache
{
    public Dictionary<string, SubSaveData> saves = new();

    public SubLibrarySaveDataCache()
    {
        OnStartedLoading += (_, __) =>
        {
            Plugin.Logger.LogInfo($"Started loading sub library save data");
        };

        OnStartedSaving += (object sender, JsonFileEventArgs args) =>
        {
            Plugin.Logger.LogInfo($"Started saving save data");
        };
    }
}
