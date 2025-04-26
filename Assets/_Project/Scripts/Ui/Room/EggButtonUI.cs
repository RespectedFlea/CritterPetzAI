using UnityEngine;
using CritterPetz;

/// <summary>
/// Handles clicking an egg button in the RoomEggSelector UI.
/// Sends the selected EggData back to RoomEggSelector.
/// </summary>
public class EggButtonUI : MonoBehaviour
{
    private EggData eggData;

    public void SetData(EggData egg)
    {
        eggData = egg;
    }

    public void OnClick()
    {
        if (RoomEggSelector.Instance != null && eggData != null)
        {
            RoomEggSelector.Instance.OnEggClicked(eggData, gameObject);
        }
    }
}
