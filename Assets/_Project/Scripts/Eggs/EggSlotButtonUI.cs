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
        if (RoomManager.Instance != null)
        {
            RoomManager.Instance.AttemptHatch(slotIndex);
        }
    }
}
