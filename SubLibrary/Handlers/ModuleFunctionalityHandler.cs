using Nautilus.Handlers;
using SubLibrary.Attributes;
using SubLibrary.Monobehaviors;
using SubLibrary.UpgradeModules;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace SubLibrary.Handlers;

public class ModuleFunctionalityHandler : MonoBehaviour
{
    public static Dictionary<TechType, Type> ModuleFunctions = new();

    [SerializeField, Tooltip("The GameObject on which module function components will be placed")] private GameObject moduleFunctionsRoot;
    [SerializeField] private ModdedUpgradeConsole[] upgradeConsoles;

    private void Start()
    {
        foreach (var item in upgradeConsoles)
        {
            item.modules.onEquip += OnModuleEquipped;
            item.modules.onUnequip += OnModuleUnequipped;
        }
    }

    /// <summary>
    /// Adds your behavior component to the <see cref="ModuleFunctions"/>. It will be enabled when the moduleTechType is in the upgrade console
    /// </summary>
    /// <param name="moduleTechType">The tech type for your module item</param>
    /// <param name="behaviorComponentType">The type of your custom behavior component. Must inherit from <see cref="MonoBehaviour"/></param>
    public static void RegisterModuleFunctions(TechType moduleTechType, Type behaviorComponentType)
    {
        ModuleFunctions.Add(moduleTechType, behaviorComponentType);
    }

    /// <summary>
    /// Registers all classes in the given assembly using the <see cref="ModdedSubModuleAttribute"/> to the <see cref="ModuleFunctions"/> dictionary.
    /// </summary>
    /// <param name="assembly">Your mod's assembly</param>
    public static void RegisterModuleFunction(Assembly assembly)
    {
        foreach (var type in assembly.GetTypes())
        {
            ModdedSubModuleAttribute attribute = type.GetCustomAttribute<ModdedSubModuleAttribute>();
            if (attribute is null || !type.IsClass || type.IsAbstract)
            {
                continue;
            }

            TechType techType;
            if (Enum.TryParse(attribute.ModuleTechType, out techType) || EnumHandler.TryGetValue(attribute.ModuleTechType, out techType))
            {
                ModuleFunctions.Add(techType, type);
            }
        }
    }

    private void OnModuleEquipped(string slot, InventoryItem item)
    {
        Type type = ModuleFunctions[item.item.GetTechType()];
        moduleFunctionsRoot.AddComponent(type);
        NotifyOnChange(item.item.GetTechType(), true);
    }

    private void OnModuleUnequipped(string slot, InventoryItem item)
    {
        Type type = ModuleFunctions[item.item.GetTechType()];
        Component component = moduleFunctionsRoot.GetComponent(type);
        (component as MonoBehaviour).enabled = false;
        Destroy(component);
        NotifyOnChange(item.item.GetTechType(), false);
    }

    private void NotifyOnChange(TechType type, bool added)
    {
        foreach (var onModuleChange in moduleFunctionsRoot.GetComponents<IOnModulesChanged>())
        {
            if ((onModuleChange as MonoBehaviour).enabled)
            {
                onModuleChange.OnChange(type, added);
            }
        }
    }
}
