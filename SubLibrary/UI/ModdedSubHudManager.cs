using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SubLibrary.UI;

internal class ModdedSubHudManager : MonoBehaviour
{
    [SerializeField] private SubRoot subRoot;
    [SerializeField, Tooltip("Can be null if you don't have a horn")] private GameObject hornObject;
    [SerializeField] private CanvasGroup canvasGroup;

    private bool hudActive;

    private List<IUIElement> uiElements;

    private void OnValidate()
    {
        if (!subRoot) subRoot = GetComponentInParent<SubRoot>();
    }

    private void Start()
    {
        canvasGroup.alpha = 0;
        uiElements = GetComponentsInChildren<IUIElement>(true).ToList();
        subRoot.BroadcastMessage("NewAlarmState");
    }

    private void Update()
    {
        if (!subRoot.LOD.IsFull()) return;

        if (subRoot.live.IsAlive())
        {
            UpdateHUD();
        }

        if (Player.main.currentSub != subRoot || subRoot.subDestroyed) return;

        float targetAlpha = hudActive ? 1 : 0;
        canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, targetAlpha, Time.deltaTime * 4f);
        canvasGroup.interactable = hudActive;
    }

    private void UpdateHUD()
    {
        foreach (IUIElement uiElement in uiElements)
        {
            uiElement.UpdateUI();
        }
    }

    protected virtual void OnSubDestroyed()
    {
        foreach (IUIElement uiElement in uiElements)
        {
            uiElement.OnSubDestroyed();
        }
    }

    /// <summary>
    /// Called via <see cref="GameObject.SendMessage(string)"/> when the player starts piloting
    /// </summary>
    private void StartPiloting()
    {
        hudActive = true;
        if (hornObject) hornObject.SetActive(true);
    }

    /// <summary>
    /// Called via <see cref="GameObject.SendMessage(string)"/> when the player stops piloting
    /// </summary>
    private void StopPiloting()
    {
        hudActive = false;
        if (hornObject) hornObject.SetActive(false);
    }
}
