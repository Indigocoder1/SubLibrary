using UnityEngine;

namespace SubLibrary.RuntimeSpawners;

internal class SpawnPowerCells : MonoBehaviour, ICyclopsReferencer
{
    [SerializeField] private Transform parent;
    [SerializeField] private Vector3 localPos;
    [SerializeField] private Vector3 localAngles;
    [SerializeField] private Vector3 localScale;
    [SerializeField] private BatterySource batterySource;

    private void OnValidate()
    {
        batterySource = GetComponent<BatterySource>();
    }

    public void OnCyclopsReferenceFinished(GameObject cyclops)
    {
        var models = LoadModels(cyclops);

        for (int i = 0; i < models.Length; i++)
        {
            batterySource.batteryModels[i].model = SpawnPowerCellModel(models[i]);
        }
    }

    private GameObject[] LoadModels(GameObject cyclops)
    {
        var powerCellModelReference = cyclops.transform.Find("cyclopspower/generator/SubPowerSocket1/SubPowerCell1/model").gameObject;
        var ionPowerCellModelReference = cyclops.transform.Find("cyclopspower/generator/SubPowerSocket1/SubPowerCell1/engine_power_cell_ion").gameObject;

        return new[]
        {
            powerCellModelReference,
            ionPowerCellModelReference,
        };
    }

    private GameObject SpawnPowerCellModel(GameObject modelReference)
    {
        var spawnedModel = Instantiate(modelReference, parent);

        spawnedModel.transform.localPosition = localPos;
        spawnedModel.transform.localEulerAngles = localAngles;
        spawnedModel.transform.localScale = localScale;

        return spawnedModel;
    }
}
