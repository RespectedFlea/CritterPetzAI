using UnityEngine;
using CritterPetz;


public class EggButtonUI : MonoBehaviour
{
    private EggData eggData;

    /// <summary>
    /// Called by RoomEggSelector after instantiating this prefab.
    /// </summary>
    /// <param name="egg">The EggData to display and represent.</param>
    public void SetData(EggData egg)
    {
        eggData = egg;
    }

    /// <summary>
    /// Called by Unity UI Button via OnClick (set in prefab).
    /// </summary>
    public void OnClick()
    {
        if (RoomEggSelector.Instance != null && eggData != null)
        {
            RoomEggSelector.Instance.OnEggClicked(eggData, gameObject);
        }
    }
}
