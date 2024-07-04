using System;
using System.Collections.Generic;

namespace SubLibrary.SaveData;

[Serializable]
public abstract class BaseSubDataClass
{

}

public class ModuleDataClass : BaseSubDataClass
{
    public Dictionary<string, Dictionary<string, TechType>> modules = new();
}
