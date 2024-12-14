using System;
using System.Collections.Generic;

namespace SubLibrary.SaveData;

[Serializable]
public abstract class BaseSubDataClass
{
    /// <summary>
    /// The fire values for a sub
    /// </summary>
    public (int fireCount, float smokeVal) fireValues = new();
}

public class ModuleDataClass : BaseSubDataClass
{
    /// <summary>
    /// The modules for a sub. The keys go like this: 
    /// First dictionary = gameObject.name of the upgrade console. Second dictionary = The slot name of the equipment.
    /// Second dictionary result: The TechType in the specified slot.
    /// </summary>
    public Dictionary<string, Dictionary<string, TechType>> modules = new();
}
