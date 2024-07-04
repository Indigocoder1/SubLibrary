using UnityEngine;

namespace SubLibrary.UI.UIManagers;

internal class EngineUIManager : MonoBehaviour, IUIElement
{
    [SerializeField] private CyclopsMotorMode motorMode;
    [SerializeField] private GameObject engineOffIndicator;

    public void UpdateUI()
    {
        engineOffIndicator.SetActive(!motorMode.engineOn);
    }

    public void OnSubDestroyed()
    {
        //Nothing extra needed here
    }
}
