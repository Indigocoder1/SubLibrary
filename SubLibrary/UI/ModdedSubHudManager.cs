using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SubLibrary.UI;

internal class ModdedSubHudManager : MonoBehaviour, IOnTakeDamage
{
    [SerializeField] private SubRoot subRoot;
    [SerializeField] private LiveMixin subLiveMixin;
    [SerializeField] private BehaviourLOD behaviourLOD;
    [SerializeField, Tooltip("Can be null if you don't have a horn")] private GameObject hornObject;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private FMOD_CustomEmitter creatureDamagesSFX;
    [SerializeField] private CyclopsNoiseManager noiseManager;

    private bool hudActive;
    private bool creatureAttackWarning;
    private bool fireWarning;
    private bool hullDamageWarning;
    private bool oldWarningState;

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

        if (subLiveMixin.IsAlive())
        {
            UpdateHUD();
        }

        hullDamageWarning = subLiveMixin.GetHealthFraction() < 0.8f;
        if (Player.main.currentSub != subRoot || subRoot.subDestroyed) return;

        float targetAlpha = hudActive ? 1 : 0;
        canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, targetAlpha, Time.deltaTime * 4f);
        canvasGroup.interactable = hudActive;

        if (creatureAttackWarning && fireWarning)
        {
            subRoot.voiceNotificationManager.PlayVoiceNotification(subRoot.creatureAttackNotification);
        }
        else if (creatureAttackWarning)
        {
            subRoot.voiceNotificationManager.PlayVoiceNotification(subRoot.creatureAttackNotification);
        }
        else if (fireWarning)
        {
            subRoot.voiceNotificationManager.PlayVoiceNotification(subRoot.fireNotification);
        }
        else if (noiseManager.GetNoisePercent() > 0.9f && !IsInvoking(nameof(PlayCavitationWarningDelayed)))
        {
            Invoke(nameof(PlayCavitationWarningDelayed), 2f);
        }
        else if (hullDamageWarning)
        {
            subRoot.voiceNotificationManager.PlayVoiceNotification(subRoot.hullDamageNotification);
        }

        if (fireWarning || creatureAttackWarning)
        {
            subRoot.subWarning = true;
        }
        else
        {
            subRoot.subWarning = false;
        }

        if (subRoot.subWarning != oldWarningState)
        {
            subRoot.BroadcastMessage("NewAlarmState", SendMessageOptions.DontRequireReceiver);
        }

        oldWarningState = subRoot.subWarning;
    }

    public bool HasFireWarning() => fireWarning;
    public bool HasCreatureAttack() => creatureAttackWarning;

    private void PlayCavitationWarningDelayed()
    {
        subRoot.voiceNotificationManager.PlayVoiceNotification(subRoot.cavitatingNotification);
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

    public void OnTakeDamage(DamageInfo damageInfo)
    {
        if (damageInfo.type == DamageType.Normal || damageInfo.type == DamageType.Electrical)
        {
            OnTakeCreatureDamage();
        }
        else if (damageInfo.type == DamageType.Collide)
        {
            OnTakeCollisionDamage(damageInfo.damage);
        }
    }

    private void OnTakeFireDamage()
    {
        CancelInvoke(nameof(ClearFireWarning));
        Invoke(nameof(ClearFireWarning), 10f);
        fireWarning = true;
    }

    private void OnTakeCreatureDamage()
    {
        CancelInvoke(nameof(ClearCreatureWarning));
        Invoke(nameof(ClearCreatureWarning), 10f);
        creatureAttackWarning = true;
        creatureDamagesSFX.Play();
        MainCameraControl.main.ShakeCamera(1.5f);
    }

    private void OnTakeCollisionDamage(float value)
    {
        value *= 1.5f;
        value = Mathf.Clamp(value / 100f, 0.5f, 1.5f);
        MainCameraControl.main.ShakeCamera(value, -1f, MainCameraControl.ShakeMode.Linear, 1f);
    }

    private void ClearCreatureWarning()
    {
        creatureAttackWarning = false;
    }

    private void ClearFireWarning()
    {
        fireWarning = false;
    }

    /// <summary>
    /// Called via <see cref="GameObject.SendMessage(string)"/> when the player starts piloting
    /// </summary>
    private void StartPiloting()
    {
        hudActive = true;
        if(hornObject) hornObject.SetActive(true);
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
