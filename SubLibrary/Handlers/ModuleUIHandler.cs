using System;
using System.Collections.Generic;
using UnityEngine.UI;

namespace SubLibrary.Handlers;

public static class ModuleUIHandler
{
    internal static List<EquipmentData> EquipmentDatas;

    /// <summary>
    /// Registers an <see cref="EquipmentData"/> to be added to <see cref="uGUI_Equipment"/>
    /// </summary>
    /// <param name="equipmentData">The equipment data to register</param>
    public static void RegisterEquipmentData(EquipmentData equipmentData)
    {
        EquipmentDatas.Add(equipmentData);
    }

    /// <summary>
    /// Handles registering upgrade console equipment slots for use in the PDA UI
    /// </summary>
    public struct EquipmentData
    {
        public string baseModuleName;
        public int numberOfModules;
        public Action<Image> onModifyBackgroundImage;

        /// <param name="baseModuleName">The base name for your equipment slot. It will have each module index appended to the end. I.e. "SubModule" → "SubModule1"</param>
        /// <param name="numberOfModules">How many module slots to make</param>
        /// <param name="onModifyBackgroundImage">Callback to modify the background image of the upgrade modules UI</param>
        public EquipmentData(string baseModuleName, int numberOfModules, Action<Image> onModifyBackgroundImage)
        {
            this.baseModuleName = baseModuleName;
            this.numberOfModules = numberOfModules;
            this.onModifyBackgroundImage = onModifyBackgroundImage;
        }
    }
}
