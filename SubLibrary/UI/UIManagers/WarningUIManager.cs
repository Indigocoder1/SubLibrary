using UnityEngine;
using UnityEngine.UI;

namespace SubLibrary.UI.UIManagers;

internal class WarningUIManager : MonoBehaviour, IUIElement
{
    [SerializeField] private ModdedSubHudManager hudManager;
    [SerializeField] private Image fireWarningIcon;
    [SerializeField] private Image creatureAttackIcon;

    public void UpdateUI()
    {
        float warningAlpha = Mathf.PingPong(Time.time * 5f, 1f);
        fireWarningIcon.color = new Color(1f, 1f, 1f, warningAlpha);
        creatureAttackIcon.color = new Color(1f, 1f, 1f, warningAlpha);

        fireWarningIcon.gameObject.SetActive(hudManager.HasFireWarning());
        creatureAttackIcon.gameObject.SetActive(hudManager.HasCreatureAttack());
    }

    public void OnSubDestroyed()
    {
        fireWarningIcon.gameObject.SetActive(false);
        creatureAttackIcon.gameObject.SetActive(false);
    }
}
