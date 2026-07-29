using UnityEngine;
using UnityEngine.UI;

namespace SubLibrary.UI.UIManagers;

internal class HealthUIManager : MonoBehaviour, IUIElement
{
    // Code credit:
    // SealSub's HealthUIManager https://github.com/32Kallies/SealSub/blob/main/SealSubMod/MonoBehaviours/UI/HealthUIManager.cs
    // Contributors:
    // - EldritchCarMaker
    
    [SerializeField] private LiveMixin subLiveMixin;
    [SerializeField] private Image healthBar;

    public void UpdateUI()
    {
        float healthFraction = subLiveMixin.GetHealthFraction();
        healthBar.fillAmount = Mathf.Lerp(healthBar.fillAmount, healthFraction, Time.deltaTime * 2f);
    }

    public void OnSubDestroyed()
    {
        healthBar.fillAmount = 0;
    }
}
