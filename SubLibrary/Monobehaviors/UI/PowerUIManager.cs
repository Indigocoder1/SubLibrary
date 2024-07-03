using SubLibrary.Interfaces;
using TMPro;
using UnityEngine;

namespace SubLibrary.Monobehaviors.UI;

internal class PowerUIManager : MonoBehaviour, IUIElement
{
    [SerializeField] private SubRoot subRoot;
    [SerializeField] private TextMeshProUGUI powerText;

    public void OnSubDestroyed()
    {
        float normalizedPower = subRoot.powerRelay.GetPower() / subRoot.powerRelay.GetMaxPower();
        int currentPower = subRoot.powerRelay.GetMaxPower() == 0f ? 0 : Mathf.CeilToInt(normalizedPower * 100f);

        powerText.text = $"{currentPower}%";
    }

    public void UpdateUI()
    {
        //Nothing extra needed here
    }
}
