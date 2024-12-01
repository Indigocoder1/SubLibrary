using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Nautilus.Handlers;
using SubLibrary.Materials;
using SubLibrary.SaveData;
using System.Reflection;

namespace SubLibrary;

[BepInPlugin(GUID, pluginName, versionString)]
[BepInDependency("com.snmodding.nautilus")]
public class Plugin : BaseUnityPlugin
{
    private const string GUID = "com.indigocoder.sublibrary";
    private const string pluginName = "Sub Library";
    private const string versionString = "1.4.2";

    public new static ManualLogSource Logger { get; private set; }

    private static Assembly Assembly { get; } = Assembly.GetExecutingAssembly();

    internal static SubLibrarySaveDataCache SubSaves { get; } = SaveDataHandler.RegisterSaveDataCache<SubLibrarySaveDataCache>();

    private void Awake()
    {
        Logger = base.Logger;

        UWE.CoroutineHost.StartCoroutine(MaterialSetter.LoadMaterialsAsync());

        // register harmony patches, if there are any
        Harmony.CreateAndPatchAll(Assembly, $"{GUID}");
        Logger.LogInfo($"Plugin {GUID} is loaded!");
    }
}