using TMPro;
using UnityEngine;

namespace SubLibrary.UI.UIManagers;

internal class PowerUIManager : MonoBehaviour, IUIElement
{
    // Code credit:
    // SealSub's PowerUIManager https://github.com/32Kallies/SealSub/blob/main/SealSubMod/MonoBehaviours/UI/PowerUIManager.cs
    // Contributors:
    // - EldritchCarMaker
    
    [SerializeField] private SubRoot subRoot;
    [SerializeField] private TextMeshProUGUI powerText;

    public void UpdateUI()
    {
        float normalizedPower = subRoot.powerRelay.GetPower() / subRoot.powerRelay.GetMaxPower();
        int currentPower = subRoot.powerRelay.GetMaxPower() == 0f ? 0 : Mathf.CeilToInt(normalizedPower * 100f);

        powerText.text = $"{currentPower}%";
    }

    public void OnSubDestroyed()
    {
        //Nothing extra needed here
    }
}
