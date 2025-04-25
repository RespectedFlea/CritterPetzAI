using System.Collections.Generic;
using UnityEngine;
using CritterPetz;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    // List of all EggData the player owns
    public List<EggData> eggInventory = new List<EggData>();

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
}
