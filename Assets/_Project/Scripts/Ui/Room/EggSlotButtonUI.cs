using UnityEngine;
using UnityEngine.UI;
using CritterPetz;

public class EggSlotButtonUI : MonoBehaviour
{
    public int slotIndex;

    private void Start()
    {
        var button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(OnClick);
        }
        else
        {
            Debug.LogError("[EggSlotButtonUI] No Button component found on EggSlot!");
        }
    }

    void OnClick()
    {
        if (RoomManager.Instance == null)
        {
            Debug.LogError("[EggSlotButton] RoomManager.Instance is NULL!");
            return;
        }

        if (RoomManager.Instance.IsAnimalLivingInSlot(slotIndex))
        {
            RoomManager.Instance.StoreAnimalFromSlot(slotIndex);
            Debug.Log($"Stored animal from slot {slotIndex}");
        }
        else if (RoomManager.Instance.IsSlotReadyToHatch(slotIndex))
        {
            if (!RoomManager.Instance.IsSlotMidHatching(slotIndex))
            {
                RoomManager.Instance.TryHatch(slotIndex);
            }
            else
            {
                Debug.Log($"[EggSlotButton] Slot {slotIndex} is mid-hatching. Ignoring click.");
            }
        }
        else if (RoomManager.Instance.IsSlotMidHatching(slotIndex))
        {
            Debug.Log($"[EggSlotButton] Slot {slotIndex} is mid-hatching. Ignoring click.");
        }
        else
        {
            if (RoomEggSelector.Instance != null)
            {
                RoomEggSelector.Instance.OpenPanelByIndex(slotIndex);
                Debug.Log($"[EggSlotButton] Opening EggSelector for slot {slotIndex}");
            }
            else
            {
                Debug.LogError("[EggSlotButton] RoomEggSelector.Instance is NULL!");
            }
        }

        Debug.Log($"Clicked Egg Slot: {slotIndex}");
    }

}
