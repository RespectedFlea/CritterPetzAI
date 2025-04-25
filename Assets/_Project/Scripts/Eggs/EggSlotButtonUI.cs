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
        if (RoomManager.Instance.IsSlotReadyToHatch(slotIndex))
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
