using UnityEngine;
using UnityEngine.UI;

namespace SubLibrary.UI.UIManagers;

internal class NoiseUIManager : MonoBehaviour, IUIElement
{
    [SerializeField] private CyclopsNoiseManager noiseManager;
    [SerializeField] private Image noiseBar;

    public void UpdateUI()
    {
        float noisePercent = noiseManager.GetNoisePercent();
        noiseBar.fillAmount = Mathf.Lerp(noiseBar.fillAmount, noisePercent, Time.deltaTime);
    }

    public void OnSubDestroyed()
    {
        //Nothing extra needed here
    }
}
