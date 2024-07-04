using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using SubLibrary.Utilities;
using System.Reflection;

namespace SubLibrary;

[BepInPlugin(GUID, pluginName, versionString)]
[BepInDependency("com.snmodding.nautilus")]
public class Plugin : BaseUnityPlugin
{
    private const string GUID = "com.indigocoder.sublibrary";
    private const string pluginName = "Sub Library";
    private const string versionString = "1.0.0";

    public new static ManualLogSource Logger { get; private set; }

    private static Assembly Assembly { get; } = Assembly.GetExecutingAssembly();

    private void Awake()
    {
        Logger = base.Logger;

        UWE.CoroutineHost.StartCoroutine(MaterialSetter.LoadMaterialsAsync());

        // register harmony patches, if there are any
        Harmony.CreateAndPatchAll(Assembly, $"{PluginInfo.PLUGIN_GUID}");
        Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
    }
}