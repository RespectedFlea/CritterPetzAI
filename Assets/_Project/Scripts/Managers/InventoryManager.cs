using System.Collections.Generic;
using UnityEngine;
using CritterPetz;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    // Simplified initialization of the list
    public List<EggData> eggInventory = new();
    // Track stored Animals too
    public List<AnimalData> animalInventory = new();


    void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    // Adds an egg to the inventory
    public void AddEgg(EggData eggData)
    {
        eggInventory.Add(eggData);
        Debug.Log($"Added egg: {eggData.eggName}");
    }


    // Removes an egg from the inventory
    public void RemoveEgg(EggData eggData)
    {
        if (eggInventory.Contains(eggData))
        {
            eggInventory.Remove(eggData);
            Debug.Log($"Removed egg: {eggData.eggName}");
        }
    }

    // Get all eggs in inventory
    public List<EggData> GetAllEggs()
    {
        return eggInventory;
    }

    // Clear all eggs (useful for debugging or resets)
    public void ClearInventory()
    {
        eggInventory.Clear();
    }
    // Adds an Animal to inventory
    public void AddAnimal(AnimalData animalData)
    {
        animalInventory.Add(animalData);
        Debug.Log($"Added animal: {animalData.animalName}");
    }

    // Removes an Animal from inventory
    public void RemoveAnimal(AnimalData animalData)
    {
        if (animalInventory.Contains(animalData))
        {
            animalInventory.Remove(animalData);
            Debug.Log($"Removed animal: {animalData.animalName}");
        }
    }

    // Get all stored Animals
    public List<AnimalData> GetAllAnimals()
    {
        return animalInventory;
    }

    // Clear all stored Animals (optional for resets)
    public void ClearAnimalInventory()
    {
        animalInventory.Clear();
    }

}
