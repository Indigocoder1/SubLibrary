using SubLibrary.Interfaces;
using UnityEngine;
using UnityEngine.UI;

namespace SubLibrary.Monobehaviors.UI;

internal class HealthUIManager : MonoBehaviour, IUIElement
{
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
