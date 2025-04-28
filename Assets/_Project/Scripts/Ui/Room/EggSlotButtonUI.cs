using UnityEngine;
using UnityEngine.UI;

public class EggSlotButtonUI : MonoBehaviour
{
    public int slotIndex;

    void Start()
    {
        GetComponent<Button>().onClick.AddListener(OnClick);
    }
    void OnClick()
    {
        // 🐾 If an Animal is living in this slot
        if (RoomManager.Instance.IsAnimalLivingInSlot(slotIndex))
        {
            RoomManager.Instance.StoreAnimalFromSlot(slotIndex);
            Debug.Log($"Stored animal from slot {slotIndex}");
        }
        else if (RoomManager.Instance.IsSlotReadyToHatch(slotIndex))
        {
            RoomManager.Instance.TryHatch(slotIndex);
        }
        else
        {
            RoomEggSelector.Instance.OpenPanelByIndex(slotIndex);
        }

        Debug.Log($"Clicked Egg Slot: {slotIndex}");
    }
}
