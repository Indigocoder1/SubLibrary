using UnityEngine;

namespace SubLibrary.UI.UIManagers;

internal class EngineUIManager : MonoBehaviour, IUIElement
{
    // Code credit:
    // SealSub's EngineUIManager https://github.com/32Kallies/SealSub/blob/main/SealSubMod/MonoBehaviours/UI/EngineUIManager.cs
    // Contributors:
    // - Kallie23
    // - EldritchCarMaker
    
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
