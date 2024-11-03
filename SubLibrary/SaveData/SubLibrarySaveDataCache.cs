using Nautilus.Json;
using Nautilus.Json.Attributes;
using System.Collections.Generic;

namespace SubLibrary.SaveData;

[FileName("SubLibrarySaveData")]
internal class SubLibrarySaveDataCache : SaveDataCache
{
    public Dictionary<string, SubSaveData> saves = new();
}
