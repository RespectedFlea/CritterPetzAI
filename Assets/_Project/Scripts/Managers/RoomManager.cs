using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CritterPetz;

public class RoomManager : MonoBehaviour
{
    public static RoomManager Instance;

    [Header("UI Slot Assignments")]
    public EggData[] equippedEggs = new EggData[3];
    public Image[] eggSlotIcons;

    [Header("World Egg Spawn Settings")]
    public GameObject eggPrefab;
    public Transform[] eggSpawnPoints = new Transform[3];
    private GameObject[] spawnedEggs = new GameObject[3];

    [Header("Hatch Settings")]
    public readonly float hatchDuration = 30f;
    private readonly float[] hatchTimers = new float[3];
    private readonly bool[] isHatching = new bool[3];
    private readonly bool[] isReadyToHatch = new bool[3];

    public Sprite defaultEmptySlotSprite;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    private void Update()
    {
        for (int i = 0; i < equippedEggs.Length; i++)
        {
            if (isHatching[i])
            {
                hatchTimers[i] -= Time.deltaTime;

                if (hatchTimers[i] <= 0)
                {
                    isHatching[i] = false;
                    isReadyToHatch[i] = true;
                    Debug.Log($"🐣 Egg in slot {i} is ready to hatch!");

                    var label = eggSlotIcons[i].GetComponentInChildren<TextMeshProUGUI>();
                    if (label != null) label.text = "Hatch!";

                    var eggObj = spawnedEggs[i];
                    if (eggObj != null && eggObj.TryGetComponent<EggComponent>(out var eggComp))
                    {
                        eggComp.StartHatchWiggle();
                    }
                }
                else
                {
                    var label = eggSlotIcons[i].GetComponentInChildren<TextMeshProUGUI>();
                    if (label != null)
                        label.text = $"{Mathf.CeilToInt(hatchTimers[i])}s";
                }
            }
        }
    }

    public void SetEquippedEgg(int index, EggData egg)
    {
        if (index < 0 || index >= equippedEggs.Length) return;

        // If swapping, return the old egg first
        if (equippedEggs[index] != null)
        {
            InventoryManager.Instance.AddEgg(equippedEggs[index]);
        }

        // Destroy the old spawned egg first
        if (spawnedEggs[index] != null)
        {
            Destroy(spawnedEggs[index]);
            spawnedEggs[index] = null;
        }

        equippedEggs[index] = egg;

        if (egg != null)
        {
            eggSlotIcons[index].sprite = egg.eggSprite;
            eggSlotIcons[index].color = Color.white;

            var label = eggSlotIcons[index].GetComponentInChildren<TextMeshProUGUI>();
            if (label != null)
                label.text = $"{Mathf.CeilToInt(hatchDuration)}s";

            StartHatchTimer(index, egg.hatchDuration);

            // Spawn the egg prefab in the world
            if (eggPrefab != null && eggSpawnPoints[index] != null)
            {
                GameObject newEgg = Instantiate(eggPrefab, eggSpawnPoints[index].position, Quaternion.identity);
                var eggComp = newEgg.GetComponent<EggComponent>();
                if (eggComp != null)
                {
                    eggComp.Initialize(egg);
                    eggComp.slotTransform = eggSpawnPoints[index];
                }
                spawnedEggs[index] = newEgg;
            }
        }
        else
        {
            ClearSlot(index, true);
        }
    }

    private void StartHatchTimer(int index, float duration)
    {
        hatchTimers[index] = duration;
        isHatching[index] = true;
        isReadyToHatch[index] = false;
    }

    public void TryHatch(int index)
    {
        if (!isReadyToHatch[index]) return;

        Debug.Log($"[RoomManager] Slot {index} hatch pressed.");

        if (spawnedEggs[index] != null)
        {
            if (spawnedEggs[index].TryGetComponent<EggComponent>(out var eggComp))
            {
                eggComp.PlayHatchAnimation(); // Only start the hatch animation
            }
        }

        // ❌ No ClearSlot here anymore — EggComponent handles destruction after animation
    }

    public void ClearSlot(int index, bool returnToInventory)
    {
        if (index < 0 || index >= equippedEggs.Length) return;

        // Destroy spawned egg if exists
        if (spawnedEggs[index] != null)
        {
            Destroy(spawnedEggs[index]);
            spawnedEggs[index] = null;
        }

        if (returnToInventory && equippedEggs[index] != null)
        {
            InventoryManager.Instance.AddEgg(equippedEggs[index]);
        }

        equippedEggs[index] = null;
        isHatching[index] = false;
        isReadyToHatch[index] = false;
        hatchTimers[index] = 0;

        eggSlotIcons[index].sprite = defaultEmptySlotSprite;
        eggSlotIcons[index].color = Color.white;

        var label = eggSlotIcons[index].GetComponentInChildren<TextMeshProUGUI>();
        if (label != null)
            label.text = "Empty";
    }

    public bool IsSlotEmpty(int index)
    {
        return equippedEggs[index] == null;
    }

    public bool IsSlotReadyToHatch(int index)
    {
        return isReadyToHatch[index];
    }

    public bool IsSlotBusy(int index)
    {
        return equippedEggs[index] != null && !isReadyToHatch[index];
    }

    public void SpawnAnimalAtSlot(Vector3 position, EggData eggData)
    {
        if (eggData == null || eggData.animalData == null)
        {
            Debug.LogError("Missing EggData or AnimalData!");
            return;
        }

        // Instantiate the AnimalPrefab
        GameObject animalObj = Instantiate(GameManager.Instance.animalPrefab, position, Quaternion.identity);

        // Assign Animal Data
        AnimalComponent animalComp = animalObj.GetComponent<AnimalComponent>();
        if (animalComp != null)
        {
            animalComp.SetupAnimal(eggData.animalData);
        }
    }
    public int GetSlotIndexFromTransform(Transform slot)
    {
        for (int i = 0; i < eggSpawnPoints.Length; i++)
        {
            if (eggSpawnPoints[i] == slot)
            {
                return i;
            }
        }
        return -1; // Not found
    }

}
