using System;

namespace SubLibrary.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public sealed class ModdedSubModuleAttribute : Attribute
{
    public ModdedSubModuleAttribute(string moduleTechType)
    {
        _moduleTechType = moduleTechType;
    }

    public string ModuleTechType
    {
        get
        {
            return _moduleTechType;
        }
    }

    private readonly string _moduleTechType;
}
