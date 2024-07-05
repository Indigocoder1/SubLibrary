using HarmonyLib;
using SubLibrary.Handlers;
using UnityEngine;
using UnityEngine.UI;

namespace SubLibrary.Patches;

[HarmonyPatch(typeof(uGUI_Equipment))]
internal class uGUI_EquipmentPatches
{
    [HarmonyPatch(nameof(uGUI_Equipment.Awake)), HarmonyPrefix]
    private static void Awake_Prefix(uGUI_Equipment __instance)
    {
        foreach (var data in ModuleUIHandler.EquipmentDatas)
        {
            uGUI_EquipmentSlot slot = CloneSlot(__instance, $"SeamothModule1", $"{data.baseModuleName}1");
            Image backgroundImage = slot.transform.Find(data.baseModuleName.Replace("Module", "")).GetComponent<Image>();

            data.onModifyBackgroundImage(backgroundImage);

            for (var i = 2; i <= data.numberOfModules; i++)
            {
                CloneSlot(__instance, $"SeamothModule{i}", $"{data.baseModuleName}{i}");
            }
        }
    }

    private static uGUI_EquipmentSlot CloneSlot(uGUI_Equipment equipmentMenu, string childName, string newSlotName)
    {
        Transform newSlot = GameObject.Instantiate(equipmentMenu.transform.Find(childName), equipmentMenu.transform);
        newSlot.name = newSlotName;
        uGUI_EquipmentSlot equipmentSlot = newSlot.GetComponent<uGUI_EquipmentSlot>();
        equipmentSlot.slot = newSlotName;
        return equipmentSlot;
    }
}
