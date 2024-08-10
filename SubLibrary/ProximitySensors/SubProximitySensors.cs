using System.Collections.Generic;
using UnityEngine;

namespace SubLibrary.ProximitySensors;

internal class SubProximitySensors : MonoBehaviour
{
    private const int NO_COLLISION = -1;

    [SerializeField] private List<SensorNode> sensorNodes = new();
    [SerializeField] private Animator uiWarningPanel;
    [SerializeField] private GameObject uiWarningIcon;
    [SerializeField] private string animatorActiveBoolName = "PanelActive";
    [SerializeField] private CyclopsMotorMode motorMode;
    [SerializeField] private FMOD_CustomEmitter proximitySound;
    [Tooltip("How long between each proximity check; reduced by 75% when an object is detected")]
    [SerializeField] private float sensorDelay = 1f;

    private bool detectedCollision;
    private bool panelActive;

    private void Start()
    {
        Player.main.playerModeChanged.AddHandler(gameObject, OnPlayerModeChange);
        foreach (var node in sensorNodes)
        {
            node.uiWarningDot.SetActive(false);
        }
    }

    private void OnPlayerModeChange(Player.Mode mode)
    {
        if (!gameObject.activeInHierarchy) return;

        if(mode == Player.Mode.Piloting)
        {
            Invoke(nameof(CheckForCollision), sensorDelay);
            return;
        }

        CancelInvoke();
        if(uiWarningPanel.GetBool(animatorActiveBoolName))
        {
            uiWarningPanel.SetBool(animatorActiveBoolName, false);
            uiWarningIcon.SetActive(false);
            foreach (var node in sensorNodes)
            {
                node.uiWarningDot.SetActive(false);
            }
        }
    }

    private void CheckForCollision()
    {
        detectedCollision = false;
        float closestDistance = NO_COLLISION;
        float pingSoundReduction = 15f;
        bool enableWarningUI = false;

        if(motorMode.engineOn)
        {
            RunSphereCasts(ref closestDistance, ref pingSoundReduction, out enableWarningUI);
        }

        if(closestDistance != NO_COLLISION)
        {
            float pingInterval = closestDistance / pingSoundReduction + 0.2f;
            if(!IsInvoking(nameof(PlayPingSound)))
            {
                InvokeRepeating(nameof(PlayPingSound), pingInterval, pingInterval);
            }
        }
        else
        {
            proximitySound.Stop();
            CancelInvoke(nameof(PlayPingSound));
        }

        float nextSensorDelay = sensorDelay;
        if(detectedCollision && motorMode.engineOn)
        {
            if(!uiWarningPanel.GetBool(animatorActiveBoolName))
            {
                uiWarningPanel.SetBool(animatorActiveBoolName, true);
            }
            nextSensorDelay *= 0.25f;
        }
        else if(uiWarningPanel.GetBool(animatorActiveBoolName))
        {
            uiWarningPanel.SetBool(animatorActiveBoolName, false);
        }

        uiWarningIcon.SetActive(enableWarningUI && motorMode.engineOn);

        foreach (var node in sensorNodes)
        {
            if(node.returnDistance != NO_COLLISION && motorMode.engineOn)
            {
                node.uiWarningDot.SetActive(true);
            }
            else
            {
                node.uiWarningDot.SetActive(false);
            }
        }

        Invoke(nameof(CheckForCollision), nextSensorDelay);
    }

    private void RunSphereCasts(ref float closestDistance, ref float pingSoundReduction, out bool enableWarningUI)
    {
        enableWarningUI = false;

        for (int i = 0; i < sensorNodes.Count; i++)
        {
            var node = sensorNodes[i];
            node.returnDistance = NO_COLLISION;

            float distance = node.checkTravelDistance;
            float radius = node.sphereCheckRadius;
            Vector3 pos = node.sensorProbe.position;
            Vector3 forward = node.sensorProbe.forward;

            //Taken from dnSpy. I assume it's the layer mask for terrain
            int layerMask = 1073741824;
            if(Physics.SphereCast(pos, radius, forward, out var hitInfo, distance, layerMask))
            {
                detectedCollision = true;
                node.returnDistance = hitInfo.distance;
                if(hitInfo.distance < distance / 4f)
                {
                    enableWarningUI = true;
                }

                if(hitInfo.distance < closestDistance || closestDistance == NO_COLLISION)
                {
                    closestDistance = hitInfo.distance;
                    pingSoundReduction = distance;
                }
            }
        }
    }

    private void PlayPingSound()
    {
        proximitySound.Play();
    }

    [System.Serializable]
    public struct SensorNode
    {
        [AssertNotNull(AssertNotNullAttribute.Options.IgnorePrefabs)] public GameObject uiWarningDot;
        [AssertNotNull(AssertNotNullAttribute.Options.IgnorePrefabs)] public Transform sensorProbe;
        public float sphereCheckRadius;
        [Tooltip("How far the spherecast will travel along the sensor probe forward vector")]
        public float checkTravelDistance;

        [HideInInspector] public float returnDistance;
    }
}
