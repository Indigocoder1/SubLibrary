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

    [Header("Gizmos")]
    [SerializeField] private bool drawGizmos;
    [SerializeField] private bool alwaysDrawGizmos;
    [SerializeField] private Color gizmoColor;

    [SerializeField, HideInInspector] private GameObject[] warningDots;
    [SerializeField, HideInInspector] private Transform[] sensorProbes;
    [SerializeField, HideInInspector] private float[] sphereRadi;
    [SerializeField, HideInInspector] private float[] travelDistances;

    private List<SensorNode> serializedNodes = new();
    private bool detectedCollision;
    private float pingInterval;

    private void OnValidate()
    {
        warningDots = new GameObject[sensorNodes.Count];
        sensorProbes = new Transform[sensorNodes.Count];
        sphereRadi = new float[sensorNodes.Count];
        travelDistances = new float[sensorNodes.Count];

        for (int i = 0; i < sensorNodes.Count; i++)
        {
            var node = sensorNodes[i];
            warningDots[i] = node.uiWarningDot;
            sensorProbes[i] = node.sensorProbe;
            sphereRadi[i] = node.sphereCheckRadius;
            travelDistances[i] = node.checkTravelDistance;
        }
    }

    private void Start()
    {
        for (int i = 0; i < warningDots.Length; i++)
        {
            serializedNodes.Add(new SensorNode(warningDots[i], sensorProbes[i], sphereRadi[i], travelDistances[i]));
        }

        Player.main.playerModeChanged.AddHandler(gameObject, new UWE.Event<Player.Mode>.HandleFunction(OnPlayerModeChange));
        foreach (var node in serializedNodes)
        {
            node.uiWarningDot.SetActive(false);
        }

        uiWarningIcon.SetActive(false);
    }

    private void OnPlayerModeChange(Player.Mode mode)
    {
        if (!gameObject.activeInHierarchy) return;

        if (mode == Player.Mode.Piloting)
        {
            Invoke(nameof(CheckForCollision), sensorDelay);
            return;
        }

        CancelInvoke();
        if (uiWarningPanel.GetBool(animatorActiveBoolName))
        {
            uiWarningPanel.SetBool(animatorActiveBoolName, false);
            uiWarningIcon.SetActive(false);
            foreach (var node in serializedNodes)
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

        if (motorMode.engineOn)
        {
            RunSphereCasts(ref closestDistance, ref pingSoundReduction, out enableWarningUI);
        }

        if (closestDistance != NO_COLLISION)
        {
            pingInterval = (closestDistance / pingSoundReduction) + 0.2f;
            if (!IsInvoking(nameof(PlayPingSound)))
            {
                Invoke(nameof(PlayPingSound), pingInterval);
            }
        }
        else
        {
            proximitySound.Stop();
            CancelInvoke(nameof(PlayPingSound));
        }

        float nextSensorDelay = sensorDelay;
        if (detectedCollision && motorMode.engineOn)
        {
            if (!uiWarningPanel.GetBool(animatorActiveBoolName))
            {
                uiWarningPanel.SetBool(animatorActiveBoolName, true);
            }
            nextSensorDelay *= 0.25f;
        }
        else if (uiWarningPanel.GetBool(animatorActiveBoolName))
        {
            uiWarningPanel.SetBool(animatorActiveBoolName, false);
        }

        uiWarningIcon.SetActive(enableWarningUI && motorMode.engineOn);

        foreach (var node in serializedNodes)
        {
            if (node.returnDistance != NO_COLLISION && motorMode.engineOn)
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

        for (int i = 0; i < serializedNodes.Count; i++)
        {
            var node = serializedNodes[i];
            node.returnDistance = NO_COLLISION;

            float distance = node.checkTravelDistance;
            float radius = node.sphereCheckRadius;
            Vector3 pos = node.sensorProbe.position;
            Vector3 forward = node.sensorProbe.forward;

            if (Physics.SphereCast(pos, radius, forward, out var hitInfo, distance))
            {
                if (hitInfo.collider.gameObject.layer != LayerID.TerrainCollider)
                {
                    serializedNodes[i] = node;
                    continue;
                }

                detectedCollision = true;
                node.returnDistance = hitInfo.distance;
                if (hitInfo.distance < distance / 4f)
                {
                    enableWarningUI = true;
                }

                if (hitInfo.distance < closestDistance || closestDistance == NO_COLLISION)
                {
                    closestDistance = hitInfo.distance;
                    pingSoundReduction = distance;
                }
            }

            serializedNodes[i] = node;
        }
    }

    private Mesh capsuleMesh;

    private void OnDrawGizmosSelected()
    {
        if (!drawGizmos) return;

        if (alwaysDrawGizmos) return;

        DrawFromGizmos();
    }

    private void OnDrawGizmos()
    {
        if (!drawGizmos) return;

        if (!alwaysDrawGizmos) return;

        DrawFromGizmos();
    }

    private void DrawFromGizmos()
    {
        if (capsuleMesh == null)
        {
            var capsule = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            capsuleMesh = capsule.GetComponent<MeshFilter>().mesh;

            DestroyImmediate(capsule);
        }

        Gizmos.color = gizmoColor;
        foreach (var node in sensorNodes)
        {
            Vector3 scale = new Vector3(node.sphereCheckRadius * 2, node.checkTravelDistance * 2, node.sphereCheckRadius * 2);
            Quaternion rotation = Quaternion.LookRotation(node.sensorProbe.up, node.sensorProbe.forward);

            Vector3 position = node.sensorProbe.position + (node.sensorProbe.forward * scale.y);

            Gizmos.DrawWireMesh(capsuleMesh, position, rotation, scale);
        }
    }

    private void PlayPingSound()
    {
        proximitySound.Play();

        Invoke(nameof(PlayPingSound), pingInterval);
    }

    [System.Serializable]
    public struct SensorNode
    {
        public GameObject uiWarningDot;
        public Transform sensorProbe;
        public float sphereCheckRadius;
        [Tooltip("How far the spherecast will travel along the sensor probe forward vector")]
        public float checkTravelDistance;

        [SerializeField, HideInInspector] public float returnDistance;

        public SensorNode(GameObject uiWarningDot, Transform sensorProbe, float sphereCheckRadius, float checkTravelDistance)
        {
            this.uiWarningDot = uiWarningDot;
            this.sensorProbe = sensorProbe;
            this.sphereCheckRadius = sphereCheckRadius;
            this.checkTravelDistance = checkTravelDistance;
        }
    }
}
