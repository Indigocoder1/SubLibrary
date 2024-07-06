using Oculus.Platform.Models;
using ProtoBuf;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SubLibrary.SubFire;

internal class ModdedSubFire : MonoBehaviour, IOnTakeDamage
{
    [SerializeField] private Transform fireSpawnsRoot;
    [SerializeField] private LiveMixin liveMixin;
    [SerializeField] private SubRoot subRoot;
    [SerializeField] private CyclopsExternalCams externalCams;
    [Tooltip("The renderer that shows when not in the sub. Should be visible from the cockpit glass so it looks like there's smoke inside.")]
    [SerializeField] private Renderer smokeImposterRenderer;
    [SerializeField] private Color smokeImpostorColor = new Color(0.2f, 0.2f, 0.2f, 1);
    [SerializeField] private AnimationCurve smokeImpostorRemap;
    [SerializeField] private FMOD_CustomEmitter fireMusic;
    [SerializeField] private CyclopsMotorMode cyclopsMotorMode;
    [SerializeField] private SubControl subControl;
    [SerializeField] private BehaviourLOD LOD;
    [SerializeField] private SubRoom engineRoom;
    [SerializeField] private float smokePerTick = 0.01f;
    [SerializeField] private float smokeFalloffPerRoom = 2f;
    [SerializeField] private float fireSuppressionSystemDuration = 30f;
    [SerializeField] private float fireSuppressionDoorLockDuration = 30f;

    private int oldFireCount;
    private int engineOverheatValue;
    private bool fireSuppressionActive;
    private CyclopsSmokeScreenFXController smokeController;
    private SubRoom currentSubRoom;
    private GameObject firePrefab;

    [NonSerialized, ProtoMember(1)] public int fireCount;
    [NonSerialized, ProtoMember(2)] public float currentSmokeVal;

    private List<SubRoom> subRooms = new();

    private void Start()
    {
        subRooms = fireSpawnsRoot.GetComponentsInChildren<SubRoom>().Where(i => i.transform.childCount == 0).ToList();

        smokeController = MainCamera.camera.GetComponent<CyclopsSmokeScreenFXController>();
        smokeController.intensity = currentSmokeVal;
        Color col = smokeImpostorColor;
        col.a = smokeImpostorRemap.Evaluate(currentSmokeVal);
        smokeImposterRenderer.material.SetColor(ShaderPropertyID._Color, col);
        if (fireCount > 0)
        {
            for (int i = 0; i < fireCount; i++)
            {
                SubRoom room = subRooms[Random.Range(0, subRooms.Count)];
                CreateFire(room);
            }
        }

        InvokeRepeating(nameof(SmokeSimulation), 3f, 3f);
        InvokeRepeating(nameof(FireSimulation), 10f, 10f);
        InvokeRepeating(nameof(EngineOverheatSimulation), 5f, 5f);
    }

    private void Update()
    {
        if (LOD.IsMinimal()) return;

        float smokeValue = subRooms.FirstOrDefault(i => i == currentSubRoom).smokeValue;
        currentSmokeVal = Mathf.Lerp(currentSmokeVal, smokeValue, Time.deltaTime / 2f);
        Color color = smokeImpostorColor;
        color.a = smokeImpostorRemap.Evaluate(currentSmokeVal);
        smokeImposterRenderer.material.SetColor(ShaderPropertyID._Color, color);
        if(Player.main.currentSub == null)
        {
            smokeImposterRenderer.enabled = true;
            if(smokeController)
            {
                smokeController.intensity = 0f;
            }
            return;
        }
        if(Player.main.currentSub == subRoot)
        {
            smokeImposterRenderer.enabled = true;
            return;
        }
        if(smokeController)
        {
            if(externalCams.GetActive())
            {
                smokeController.intensity = 0;
                smokeImposterRenderer.enabled = true;
                return;
            }
            smokeController.intensity = currentSmokeVal;
            smokeImposterRenderer.enabled = false;
        }
    }

    private void SmokeSimulation()
    {
        if (LOD.IsMinimal()) return;

        int fireCount = RecalcFireValues();
        foreach (var room in subRooms)
        {
            if(fireCount == 0 || fireSuppressionActive)
            {
                float fadeSpeed = fireSuppressionActive ? 45f : 15f;
                room.smokeValue = Mathf.Lerp(room.smokeValue, 0f, Time.deltaTime * fadeSpeed);
            }
            else
            {
                RecursiveIterateSmoke(new List<SubRoom>(), room, 0, room.fireValue);
                if(Player.main.currentSub == subRoot && room.smokeValue > 0.5f)
                {
                    Player.main.GetComponent<LiveMixin>().TakeDamage(0.2f, transform.position, DamageType.Smoke);
                }
            }
        }
    }

    private void FireSimulation()
    {
        if (LOD.IsMinimal()) return;

        int fireCount = RecalcFireValues();
        if (fireCount > 0 && Player.main.currentSub == subRoot)
        {
            if (!fireMusic.playing)
            {
                fireMusic.Play();
            }
        }
        else
        {
            fireMusic.Stop();
        }

        if (fireCount == 0)
        {
            if (oldFireCount > 0)
            {
                subRoot.voiceNotificationManager.PlayVoiceNotification(subRoot.fireExtinguishedNotification);
            }
            oldFireCount = 0;
            return;
        }

        float damage = fireCount * 15f;
        liveMixin.TakeDamage(damage, type: DamageType.Fire);
        BroadcastMessage("OnTakeFireDamage", SendMessageOptions.DontRequireReceiver);
        oldFireCount = fireCount;
    }

    private void EngineOverheatSimulation()
    {
        if (!LOD.IsFull()) return;

        if(cyclopsMotorMode.cyclopsMotorMode == CyclopsMotorMode.CyclopsMotorModes.Flank && subControl.appliedThrottle && cyclopsMotorMode.engineOn)
        {
            engineOverheatValue = Mathf.Min(engineOverheatValue + 1, 10);
            int fireChance = 0;
            if(engineOverheatValue > 5)
            {
                fireChance = Random.Range(1, 4);
                subRoot.voiceNotificationManager.PlayVoiceNotification(subRoot.engineOverheatCriticalNotification);
            }
            else if(engineOverheatValue > 3)
            {
                fireChance = Random.Range(1, 6);
                subRoot.voiceNotificationManager.PlayVoiceNotification(subRoot.engineOverheatNotification);
            }

            if(fireChance == 1)
            {
                CreateFire(engineRoom);
            }
        }
        else
        {
            if(cyclopsMotorMode.cyclopsMotorMode == CyclopsMotorMode.CyclopsMotorModes.Flank)
            {
                engineOverheatValue = Mathf.Max(1, engineOverheatValue - 1);
                return;
            }
            engineOverheatValue = Mathf.Max(0, engineOverheatValue - 1);
        }
    }

    private void RecursiveIterateSmoke(List<SubRoom> iteratedRooms, SubRoom room, int roomsAwayFromRoot, int baseFireValue)
    {
        if (iteratedRooms.Contains(room)) return;

        foreach (var subRoom in subRooms)
        {
            if(subRoom == room)
            {
                float fireValueFalloff = smokeFalloffPerRoom * (roomsAwayFromRoot == 0f ? 1f : (smokeFalloffPerRoom * roomsAwayFromRoot));
                subRoom.smokeValue += smokePerTick * baseFireValue / fireValueFalloff;
                subRoom.smokeValue = Mathf.Clamp01(subRoom.smokeValue);

                roomsAwayFromRoot++;
                iteratedRooms.Add(subRoom);
                foreach (var linkedRoom in subRoom.GetLinkedRooms())
                {
                    RecursiveIterateSmoke(iteratedRooms, linkedRoom, roomsAwayFromRoot, baseFireValue);
                }
                break;
            }
        }
    }

    public void ActivateFireSuppressionSystem()
    {
        StartCoroutine(StartFireSuppression());
    }

    private IEnumerator StartFireSuppression()
    {
        subRoot.voiceNotificationManager.PlayVoiceNotification(subRoot.fireSupressionNotification);
        yield return new WaitForSeconds(3f);

        fireSuppressionActive = true;
        subRoot.fireSuppressionState = true;
        subRoot.BroadcastMessage("NewAlarmState", SendMessageOptions.DontRequireReceiver);
        InvokeRepeating(nameof(FireSuppressionIteration), 0f, 2f);
        Invoke(nameof(CancelFireSuppression), fireSuppressionSystemDuration);

        gameObject.BroadcastMessage("TemporaryClose", fireSuppressionDoorLockDuration, SendMessageOptions.DontRequireReceiver);
        gameObject.BroadcastMessage("TemporaryLock", fireSuppressionDoorLockDuration, SendMessageOptions.DontRequireReceiver);
    }

    private void FireSuppressionIteration()
    {
        if (RecalcFireValues() == 0) return;

        foreach (var fire in fireSpawnsRoot.GetComponentsInChildren<Fire>())
        {
            if (fire != null)
            {
                fire.Douse(20f);
            }
        }
    }

    private void CancelFireSuppression()
    {
        fireSuppressionActive = false;
        subRoot.fireSuppressionState = false;
        subRoot.BroadcastMessage("NewAlarmState", null, SendMessageOptions.DontRequireReceiver);
        CancelInvoke(nameof(FireSuppressionIteration));
    }

    private int RecalcFireValues()
    {
        foreach (var room in subRooms)
        {
            fireCount = 0;
            Transform[] spawnNodes = room.GetSpawnNodes();
            for (int i = 0; i < spawnNodes.Length; i++)
            {
                if (spawnNodes[i].childCount != 0)
                {
                    fireCount++;
                }
            }
            room.fireValue = fireCount;
        }
        int numFires = 0;
        foreach (var subRoom in subRooms)
        {
            numFires += subRoom.fireValue;
        }

        if(numFires == 0)
        {
            BroadcastMessage("ClearFireWarning", SendMessageOptions.DontRequireReceiver);
        }

        return numFires;
    }

    public void SetPlayerRoom(SubRoom room)
    {
        currentSubRoom = room;
    }

    private void CreateFire(SubRoom startInRoom)
    {
        List<Transform> availableNodes = new();
        foreach (var node in startInRoom.GetSpawnNodes())
        {
            if (node.childCount != 0) continue;

            availableNodes.Add(node.transform);
        }

        if (availableNodes.Count == 0) return;

        int index = Random.Range(0, availableNodes.Count);
        var selectedNode = availableNodes[index];
        startInRoom.fireValue++;

        PrefabSpawnBase spawnBase = selectedNode.GetComponent<PrefabSpawnBase>();
        if (spawnBase == null) return;

        spawnBase.SpawnManual(delegate (GameObject fireGO)
        {
            Fire fire = fireGO.GetComponentInChildren<Fire>();
            if(fire)
            {
                fire.fireSubRoot = subRoot;
            }
        });
    }

    public void OnTakeDamage(DamageInfo damageInfo)
    {
        if (damageInfo.damage <= 0) return;

        float fireChance = 9f;
        if(damageInfo.type == DamageType.Fire)
        {
            fireChance = 2f;
        }

        if (!CanCreateFire(fireChance)) return;

        int roomIndex = Random.Range(0, subRooms.Count);
        CreateFire(subRooms[roomIndex]);
    }

    public int GetFireCount()
    {
        fireCount = 0;
        foreach (var subRoom in subRooms)
        {
            var spawnNodes = subRoom.GetSpawnNodes();
            for (int i = 0; i < spawnNodes.Length; i++)
            {
                if (spawnNodes[i].childCount != 0) fireCount++;
            }
        }

        return fireCount;
    }

    public List<GameObject> GetAllFires()
    {
        List<GameObject> fires = new();
        foreach (var room in subRooms)
        {
            foreach (var node in room.GetSpawnNodes())
            {
                if (node.childCount <= 0) continue;

                foreach (Transform child in node)
                {
                    fires.Add(child.gameObject);
                }
            }
        }

        return fires;
    }

    private bool CanCreateFire(float fireChance)
    {
        if (liveMixin.GetHealthFraction() > 0.8f) return false;

        float fullHealth = liveMixin.GetHealthFraction() * 100f;
        return Random.Range(0f, fullHealth) <= fireChance;
    }

    /// <summary>
    /// Called via <see cref="GameObject.SendMessage(string)"/> on a <see cref="SubRoot"/> when it's destroyed
    /// </summary>
    public void CyclopsDeathEvent()
    {
        smokeImposterRenderer.gameObject.SetActive(false);
        fireMusic.Stop();
        CancelInvoke();
    }

    private void OnDestroy()
    {
        Destroy(smokeImposterRenderer.material);
    }
}
