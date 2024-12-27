using UnityEngine;

namespace SubLibrary.Monobehaviors;

internal class SubStatusAlerter : MonoBehaviour, IOnTakeDamage
{
    [SerializeField] private SubRoot subRoot;
    [SerializeField] private FMOD_CustomEmitter creatureDamagesSFX;
    [SerializeField] private float minCollisionDamageForHUDShake;

    private bool creatureAttackWarning;
    private bool fireWarning;
    private bool hullDamageWarning;
    private bool oldWarningState;

    private void Update()
    {
        if (!subRoot.LOD.IsFull()) return;

        hullDamageWarning = subRoot.live.GetHealthFraction() < 0.8f;

        if (HasPriorityNotification(out var notification))
        {
            subRoot.voiceNotificationManager.PlayVoiceNotification(notification);
        }
        else if (subRoot.noiseManager.GetNoisePercent() > 0.9f && !IsInvoking(nameof(PlayCavitationWarningDelayed)))
        {
            Invoke(nameof(PlayCavitationWarningDelayed), 2f);
        }

        subRoot.subWarning = fireWarning || creatureAttackWarning;

        if (subRoot.subWarning != oldWarningState)
        {
            subRoot.BroadcastMessage("NewAlarmState");
        }

        oldWarningState = subRoot.subWarning;
    }

    public bool HasFireWarning() => fireWarning;
    public bool HasCreatureAttack() => creatureAttackWarning;

    private void PlayCavitationWarningDelayed()
    {
        subRoot.voiceNotificationManager.PlayVoiceNotification(subRoot.cavitatingNotification);
    }

    /// <summary>
    /// Gets the highest priority voice notification
    /// </summary>
    /// <param name="notification">The notification</param>
    /// <returns>If a notification is needed</returns>
    private bool HasPriorityNotification(out VoiceNotification notification)
    {
        bool needsNotif = false;
        notification = null;

        if (creatureAttackWarning && fireWarning)
        {
            notification = subRoot.creatureAttackNotification;
            needsNotif = true;
        }
        else if (creatureAttackWarning)
        {
            notification = subRoot.creatureAttackNotification;
            needsNotif = true;
        }
        else if (fireWarning)
        {
            notification = subRoot.fireNotification;
        }
        else if (hullDamageWarning)
        {
            notification = subRoot.hullDamageNotification;
            needsNotif = true;
        }

        return needsNotif;
    }

    public void OnTakeDamage(DamageInfo damageInfo)
    {
        if (damageInfo.dealer != null && damageInfo.dealer.CompareTag("Creature"))
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
        MainCameraControl.main.ShakeCamera(1.5f);

        if (creatureDamagesSFX.gameObject.activeSelf)
        {
            creatureDamagesSFX.Play();
        }
    }

    private void OnTakeCollisionDamage(float value)
    {
        if (value < minCollisionDamageForHUDShake) return;

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
}
