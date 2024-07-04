using System;
using System.Collections.Generic;

namespace SubLibrary.Monobehaviors.Saving;

[Serializable]
public class BaseModdedSubSaveData
{
    public Dictionary<string, Dictionary<string, TechType>> modules = new();
}
