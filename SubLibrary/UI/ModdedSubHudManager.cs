using SubLibrary.Interfaces;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace SubLibrary.Monobehaviors.UI;

internal class ModdedSubHudManager : MonoBehaviour
{
    [SerializeField] private SubRoot subRoot;
    [SerializeField] private LiveMixin subLiveMixin;
    [SerializeField] private BehaviourLOD behaviourLOD;
    [SerializeField, Tooltip("Can be null if you don't have a horn")] private GameObject hornObject;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Image creatureAttackSprite;
    [SerializeField] private FMOD_CustomEmitter creatureDamagesSFX;

    private bool hudActive;
    private bool creatureAttackWarning;
    private bool oldWarningState;
    private float warningAlpha;

    private List<IUIElement> uiElements;

    private void OnValidate()
    {
        if (!subRoot) subRoot = GetComponentInParent<SubRoot>();
        if (!subLiveMixin) subLiveMixin = GetComponentInParent<LiveMixin>();
        if (!behaviourLOD) behaviourLOD = GetComponentInParent<BehaviourLOD>();
    }

    private void Start()
    {
        canvasGroup.alpha = 0;
        uiElements = GetComponentsInChildren<IUIElement>(true).ToList();
    }

    private void Update()
    {
        if (!behaviourLOD.IsFull()) return;

        if(subLiveMixin.IsAlive())
        {
            UpdateHUD();
            creatureAttackSprite.gameObject.SetActive(creatureAttackWarning);
        }

        if (Player.main.currentSub != subRoot || subRoot.subDestroyed) return;

        float targetAlpha = hudActive ? 1 : 0;
        canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, targetAlpha, Time.deltaTime * 4f);
        canvasGroup.interactable = hudActive;

        if(creatureAttackWarning)
        {
            subRoot.voiceNotificationManager.PlayVoiceNotification(subRoot.creatureAttackNotification);
            subRoot.subWarning = true; //NOTE: COME BACK TO LATER WHEN/IF IMPLEMENTING FIRE SYSTEM
        }
        else
        {
            subRoot.subWarning = false;
        }

        warningAlpha = Mathf.PingPong(Time.time * 5f, 1f);
        creatureAttackSprite.color = new Color(1f, 1f, 1f, warningAlpha);

        if(subRoot.subWarning != oldWarningState)
        {
            subRoot.BroadcastMessage("NewAlarmState");
        }

        oldWarningState = subRoot.subWarning;
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

    public void OnTakeCreatureDamage()
    {
        CancelInvoke(nameof(ClearCreatureWarning));
        Invoke(nameof(ClearCreatureWarning), 10f);
        creatureAttackWarning = true;
        creatureDamagesSFX.Play();
        MainCameraControl.main.ShakeCamera(1.5f);
    }

    private void ClearCreatureWarning()
    {
        creatureAttackWarning = false;
    }

    private void StartPiloting()
    {
        hudActive = true;
    }

    private void StopPiloting()
    {
        hudActive = false;
    }
}
