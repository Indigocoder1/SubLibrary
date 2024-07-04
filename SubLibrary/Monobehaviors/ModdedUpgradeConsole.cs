using SubLibrary.SaveData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SubLibrary.Monobehaviors;

public class ModdedUpgradeConsole : HandTarget, IHandTarget, ISaveDataListener
{
    public Equipment modules { get; private set; }

    [SerializeField] private SubSerializationManager serializationManager;
    [Tooltip("This is the transform module prefabs will be parented to. It should have a child object identifier on it.")]
    [SerializeField] private GameObject modulesRoot;
    [SerializeField, Tooltip("This will be where the module models spawn")] private GameObject[] moduleModels;
    [SerializeField, Tooltip("The names of the module slots. I.e. SubUpgrade4. Cannot have duplicates")] private string[] slots;

    [Header("Optional")]
    [Tooltip("Localized key for the label when opening the modules. Default in English is \"Upgrade Modules\"")]
    [SerializeField] private string equipmentTextKey = "CyclopsUpgradesStorageLabel";
    [Tooltip("Localized key for the text when hovering over the upgrade console. Default in English is \"Access upgrades\"")]
    [SerializeField] private string hoverTextKey = "UpgradeConsole";

    private void OnValidate()
    {
        if (!serializationManager && GetComponentInParent<SubSerializationManager>() != null) serializationManager = GetComponentInParent<SubSerializationManager>();
    }

    public override void Awake()
    {
        base.Awake();
        if (modules == null)
        {
            InitializeModules();
        }
    }

    private void InitializeModules()
    {
        modules = new Equipment(gameObject, modulesRoot.transform);
        modules.SetLabel(equipmentTextKey);
        UpdateVisuals();
        modules.onEquip += OnEquip;
        modules.onUnequip += OnUnequip;
        modules.AddSlots(slots);
    }

    protected virtual void OnEquip(string slot, InventoryItem item)
    {
        UpdateVisuals();
    }

    protected virtual void OnUnequip(string slot, InventoryItem item)
    {
        UpdateVisuals();
    }

    protected virtual void UpdateVisuals()
    {
        for (int i = 0; i < moduleModels.Length; i++)
        {
            SetModuleVisibility(slots[i], moduleModels[i]);
        }
    }

    private void SetModuleVisibility(string slot, GameObject module)
    {
        if (!module) return;

        module.SetActive(modules.GetTechTypeInSlot(slot) != TechType.None);
    }

    public void OnHandClick(GUIHand hand)
    {
        var pda = Player.main.GetPDA();
        Inventory.main.SetUsedStorage(modules);
        pda.Open(PDATab.Inventory);
    }

    public void OnHandHover(GUIHand hand)
    {
        var main = HandReticle.main;
        main.SetText(HandReticle.TextType.Hand, hoverTextKey, true, GameInput.Button.LeftHand);
        main.SetText(HandReticle.TextType.HandSubscript, string.Empty, false, GameInput.Button.None);
        main.SetIcon(HandReticle.IconType.Hand, 1f);
    }

    public void OnSaveDataLoaded(BaseSubDataClass saveData)
    {
        if (!(saveData as ModuleDataClass).modules.TryGetValue(gameObject.name, out var value)) return;

        StartCoroutine(SpawnSavedModules(value));
    }

    private IEnumerator SpawnSavedModules(Dictionary<string, TechType> cachedModules)
    {
        if (modules == null)
        {
            InitializeModules();
        }

        foreach (var module in cachedModules)
        {
            if (module.Value == TechType.None) continue;

            var task = CraftData.GetPrefabForTechTypeAsync(module.Value);
            yield return task;

            GameObject newModule = Instantiate(task.GetResult(), modulesRoot.transform);
            newModule.SetActive(false);
            modules.AddItem(module.Key, new InventoryItem(newModule.GetComponent<Pickupable>()), true);
        }
    }

    public void OnBeforeDataSaved(ref BaseSubDataClass saveData)
    {
        var newModules = new Dictionary<string, TechType>();

        foreach (var item in modules.equipment)
        {
            newModules.Add(item.Key, item.Value != null ? item.Value.item.GetTechType() : TechType.None);
        }

        (saveData as ModuleDataClass).modules[gameObject.name] = newModules;
    }
}
