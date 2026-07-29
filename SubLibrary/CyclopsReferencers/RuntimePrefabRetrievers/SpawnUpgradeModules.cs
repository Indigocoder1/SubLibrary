using UnityEngine;

namespace SubLibrary.CyclopsReferencers.RuntimePrefabRetrievers;

internal class SpawnUpgradeModules : MonoBehaviour, ICyclopsReferencer
{
    // Code credit:
    // SealSub's SealUpgradeModuleModelSpawner https://github.com/32Kallies/SealSub/blob/acafaae6573625c695642439530c93c2280ebf03/SealSubMod/MonoBehaviours/Prefab/SealUpgradeModuleModelSpawner.cs#L15
    // Contributors:
    // - Kallie23
    // - EldritchCarMaker
    
    [SerializeField] private Transform[] moduleSlots;

    public void OnCyclopsReferenceFinished(GameObject cyclops)
    {
        var upgradeModuleModel = cyclops.transform
            .Find("CyclopsMeshStatic/undamaged/cyclops_LOD0/cyclops_engine_room/cyclops_engine_console/" +
            "Submarine_engine_GEO/submarine_engine_console_01_wide/engine_console_key_01_01")
            .gameObject;

        foreach (var slot in moduleSlots)
        {
            var clone = Instantiate(upgradeModuleModel, slot, false);
            clone.transform.localPosition = new Vector3(-0.41f, -0.69f, -2.69f);
            clone.transform.localEulerAngles = Vector3.zero;
            clone.transform.localScale = Vector3.one * 2.7f;
            clone.gameObject.SetActive(true);
        }
    }
}
