using ProtoBuf;
using SubLibrary.SaveData;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SubLibrary.SubFire;

public class ModdedSubFire : MonoBehaviour, IOnTakeDamage, ISaveDataListener, ILateSaveDataListener
{
    [SerializeField, Tooltip("Should have multiple \"SubRoom\"s as children for fire spreading")] private Transform fireSpawnsRoot;
    [SerializeField] private PrefabIdentifier identifier;
    [SerializeField] private LiveMixin liveMixin;
    [SerializeField] private SubRoot subRoot;
    [SerializeField] private CyclopsExternalCams externalCams;
    [Tooltip("The renderer that shows smoke when not in the sub. Should be visible from the cockpit glass")]
    [SerializeField] private Renderer[] smokeImposterRenderers;
    [SerializeField, Tooltip("The color of the impostor smoke")] private Color smokeImpostorColor = new Color(0.2f, 0.2f, 0.2f, 1);
    [Tooltip("The opacity of the smoke impostor relative to the sub's smoke value (1 = max, 0 = none)")]
    [SerializeField] private AnimationCurve smokeImpostorRemap;
    [SerializeField, Tooltip("The music that plays when a fire is happening")] private FMOD_CustomEmitter fireMusic;
    [SerializeField] private CyclopsMotorMode cyclopsMotorMode;
    [SerializeField] private SubControl subControl;
    [SerializeField] private BehaviourLOD LOD;
    [Tooltip("The SubRoom that the engine is in. Will be the spawn point of a fire if the engine overheats")]
    [SerializeField] private SubRoom engineRoom;
    [SerializeField, Tooltip("How much smoke to add per smoke sim update")] private float smokePerTick = 0.01f;
    [SerializeField, Tooltip("How much the smoke value increment decreases per room from fire source")] private float smokeFalloffPerRoom = 2f;
    [SerializeField, Tooltip("How long the fire suppression system runs for")] private float fireSuppressionSystemDuration = 30f;
    [Tooltip("How long the doors are locked for after fire suppression is started")]
    [SerializeField] private float fireSuppressionDoorLockDuration = 30f;

    private int oldFireCount;
    private int engineOverheatValue;
    private bool fireSuppressionActive;
    private bool isSubDead;
    private CyclopsSmokeScreenFXController smokeController;
    private SubRoom currentSubRoom;

    public int fireCount;
    public float currentSmokeVal;

    private List<SubRoom> subRooms = new();

    private void Awake()
    {
        smokeController = MainCamera.camera.GetComponent<CyclopsSmokeScreenFXController>();

        subRooms = fireSpawnsRoot.GetComponentsInChildren<SubRoom>().ToList();
    }

    private void Start()
    {
        smokeController.intensity = currentSmokeVal;
        Color col = smokeImpostorColor;
        col.a = smokeImpostorRemap.Evaluate(currentSmokeVal);
        smokeImposterRenderers.ForEach(r => r.material.SetColor(ShaderPropertyID._Color, col));
        
        currentSubRoom = engineRoom;

        InvokeRepeating(nameof(SmokeSimulation), 3f, 3f);
        InvokeRepeating(nameof(FireSimulation), 10f, 10f);
        InvokeRepeating(nameof(EngineOverheatSimulation), 5f, 5f);
    }

    private void Update()
    {
        if (isSubDead) return;
        if (LOD.IsMinimal()) return;

        float smokeValue = subRooms.FirstOrDefault(i => i == currentSubRoom).smokeValue;
        currentSmokeVal = Mathf.Lerp(currentSmokeVal, smokeValue, Time.deltaTime / 2f);
        Color color = smokeImpostorColor;
        color.a = smokeImpostorRemap.Evaluate(currentSmokeVal);
        smokeImposterRenderers.ForEach(r => r.material.SetColor(ShaderPropertyID._Color, color));
        if (Player.main.currentSub == null)
        {
            smokeImposterRenderers.ForEach(r => r.enabled = true);
            if (smokeController)
            {
                smokeController.intensity = 0f;
            }
            return;
        }
        if (Player.main.currentSub != subRoot)
        {
            smokeImposterRenderers.ForEach(r => r.enabled = true);
            return;
        }
        if (smokeController)
        {
            if (externalCams.GetActive())
            {
                smokeController.intensity = 0;
                smokeImposterRenderers.ForEach(r => r.enabled = true);
                return;
            }
            smokeController.intensity = currentSmokeVal;
            smokeImposterRenderers.ForEach(r => r.enabled = false);
        }
    }

    private void SmokeSimulation()
    {
        if (LOD.IsMinimal()) return;

        int fireCount = RecalcFireValues();
        foreach (var room in subRooms)
        {
            if (fireCount == 0 || fireSuppressionActive)
            {
                float fadeSpeed = fireSuppressionActive ? 45f : 15f;
                room.smokeValue = Mathf.Lerp(room.smokeValue, 0f, Time.deltaTime * fadeSpeed);
            }
            else
            {
                RecursiveIterateSmoke(new List<SubRoom>(), room, 0, room.fireValue);
                if (Player.main.currentSub == subRoot && room.smokeValue > 0.5f)
                {
                    Player.main.GetComponent<LiveMixin>().TakeDamage(0.2f, transform.position, DamageType.Smoke);
                }
            }
        }
    }

    private void FireSimulation()
    {
        if (LOD.IsMinimal())
        {
            fireMusic.Stop();
            return;
        }

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
        subRoot.BroadcastMessage("OnTakeFireDamage");
        oldFireCount = fireCount;
    }

    private void EngineOverheatSimulation()
    {
        if (!LOD.IsFull()) return;

        if (cyclopsMotorMode.cyclopsMotorMode == CyclopsMotorMode.CyclopsMotorModes.Flank && subControl.appliedThrottle && cyclopsMotorMode.engineOn)
        {
            engineOverheatValue = Mathf.Min(engineOverheatValue + 1, 10);
            int fireChance = 0;
            if (engineOverheatValue > 5)
            {
                fireChance = Random.Range(1, 4);
                subRoot.voiceNotificationManager.PlayVoiceNotification(subRoot.engineOverheatCriticalNotification);
            }
            else if (engineOverheatValue > 3)
            {
                fireChance = Random.Range(1, 6);
                subRoot.voiceNotificationManager.PlayVoiceNotification(subRoot.engineOverheatNotification);
            }

            if (fireChance == 1)
            {
                CreateFire(engineRoom);
            }
        }
        else
        {
            if (cyclopsMotorMode.cyclopsMotorMode == CyclopsMotorMode.CyclopsMotorModes.Flank)
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
            if (subRoom == room)
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
        subRoot.BroadcastMessage("NewAlarmState");
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
        subRoot.BroadcastMessage("NewAlarmState");
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

        if (numFires == 0)
        {
            BroadcastMessage("ClearFireWarning");
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
            if (fire)
            {
                fire.fireSubRoot = subRoot;
            }
        });
    }

    public void OnTakeDamage(DamageInfo damageInfo)
    {
        if (damageInfo.damage <= 0) return;

        float fireChance = 9f;
        if (damageInfo.type == DamageType.Fire)
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
        smokeImposterRenderers.ForEach(r => r.gameObject.SetActive(false));
        fireMusic.Stop();
        CancelInvoke();

        isSubDead = true;
    }

    private void OnDestroy()
    {
        smokeImposterRenderers.ForEach(r => Destroy(r.material));
    }

    public void OnLateSaveDataLoaded(BaseSubDataClass saveData)
    {
        if (saveData == null)
        {
            saveData = new ModuleDataClass();
        }

        fireCount = saveData.fireValues.fireCount;
        currentSmokeVal = saveData.fireValues.smokeVal;

        if (fireCount > 0)
        {
            UWE.CoroutineHost.StartCoroutine(SpawnSavedFires());
        }
    }

    private IEnumerator SpawnSavedFires()
    {
        yield return new WaitForEndOfFrame();

        for (int i = 0; i < fireCount; i++)
        {
            SubRoom room = subRooms[Random.Range(0, subRooms.Count)];
            room.smokeValue = currentSmokeVal / fireCount;
            CreateFire(room);
        }
    }

    public void OnSaveDataLoaded(BaseSubDataClass saveData) { }

    public void OnBeforeDataSaved(ref BaseSubDataClass saveData)
    {
        if (saveData == null)
        {
            saveData = new ModuleDataClass();
        }

        saveData.fireValues = (GetFireCount(), currentSmokeVal);
    }
}
