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

    [Header("Hatch Settings")]
    public readonly float hatchDuration = 30f;
    private readonly float[] hatchTimers = new float[3];
    private readonly bool[] isHatching = new bool[3];
    private readonly bool[] isReadyToHatch = new bool[3];

    public Sprite defaultEmptySlotSprite;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    void Update()
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

        equippedEggs[index] = egg;

        if (egg != null)
        {
            eggSlotIcons[index].sprite = egg.eggSprite;
            eggSlotIcons[index].color = Color.white;

            var label = eggSlotIcons[index].GetComponentInChildren<TextMeshProUGUI>();
            if (label != null)
                label.text = $"{Mathf.CeilToInt(hatchDuration)}s";

            StartHatchTimer(index, egg.hatchDuration);

        }
        else
        {
            ClearSlot(index, true);
        }
    }


    void StartHatchTimer(int index, float duration)
    {
        hatchTimers[index] = duration;
        isHatching[index] = true;
        isReadyToHatch[index] = false;
    }


    public void TryHatch(int index)
    {
        if (!isReadyToHatch[index]) return;

        Debug.Log($"[RoomManager] Slot {index} hatched and egg destroyed.");
        ClearSlot(index, false); // ❌ Do not return hatched eggs to inventory
    }

    public void ClearSlot(int index, bool returnToInventory)
    {
        if (index < 0 || index >= equippedEggs.Length) return;

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
}
