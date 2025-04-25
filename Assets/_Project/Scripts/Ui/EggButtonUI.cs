using UnityEngine;
using CritterPetz;

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
