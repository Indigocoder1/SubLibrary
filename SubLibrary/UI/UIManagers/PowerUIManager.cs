using SubLibrary.Interfaces;
using TMPro;
using UnityEngine;

namespace SubLibrary.UI.UIManagers;

internal class PowerUIManager : MonoBehaviour, IUIElement
{
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
