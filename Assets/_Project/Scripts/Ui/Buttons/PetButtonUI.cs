using UnityEngine;
using CritterPetz;

/// <summary>
/// Handles clicking a Pet button in the RoomEggSelector UI.
/// Sends the selected AnimalData back to RoomEggSelector.
/// </summary>
public class PetButtonUI : MonoBehaviour
{
    private AnimalData animalData;

    public void SetData(AnimalData animal)
    {
        animalData = animal;
    }

    public void OnClick()
    {
        if (RoomEggSelector.Instance != null && animalData != null)
        {
            RoomEggSelector.Instance.OnPetClicked(animalData, gameObject);
        }
    }
}
