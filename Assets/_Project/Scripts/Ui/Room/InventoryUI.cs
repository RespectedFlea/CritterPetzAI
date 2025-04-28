using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using CritterPetz;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    public GameObject inventoryPanel;
    public GameObject buttonPrefab;
    public Transform buttonContainer;

    public void ToggleInventory()
    {
        bool isActive = !inventoryPanel.activeSelf;
        inventoryPanel.SetActive(isActive);
        if (isActive) RefreshInventory();
    }

    void RefreshInventory()
    {
        Debug.Log("Refreshing Inventory...");

        if (buttonPrefab == null || buttonContainer == null || inventoryPanel == null)
        {
            Debug.LogError("[InventoryUI] Missing references!");
            return;
        }

        foreach (Transform child in buttonContainer)
        {
            Destroy(child.gameObject);
        }

        // 🥚 Display Eggs
        Dictionary<EggData, int> eggCounts = new();
        foreach (EggData egg in InventoryManager.Instance.GetAllEggs())
        {
            if (eggCounts.ContainsKey(egg))
                eggCounts[egg]++;
            else
                eggCounts[egg] = 1;
        }

        foreach (KeyValuePair<EggData, int> pair in eggCounts)
        {
            EggData egg = pair.Key;
            int count = pair.Value;

            GameObject newButton = Instantiate(buttonPrefab, buttonContainer);

            var text = newButton.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            if (text != null)
                text.text = $"{egg.eggName} x{count}";

            var images = newButton.GetComponentsInChildren<Image>();
            foreach (Image img in images)
            {
                if (img.gameObject.name == "Icon")
                    img.sprite = egg.eggSprite;
            }
        }

        // 🐾 Display Animals
        Dictionary<AnimalData, int> animalCounts = new();
        foreach (AnimalData animal in InventoryManager.Instance.GetAllAnimals())
        {
            if (animalCounts.ContainsKey(animal))
                animalCounts[animal]++;
            else
                animalCounts[animal] = 1;
        }

        foreach (KeyValuePair<AnimalData, int> pair in animalCounts)
        {
            AnimalData animal = pair.Key;
            int count = pair.Value;

            GameObject newButton = Instantiate(buttonPrefab, buttonContainer);

            var text = newButton.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            if (text != null)
                text.text = $"{animal.animalName} x{count}";

            var images = newButton.GetComponentsInChildren<Image>();
            foreach (Image img in images)
            {
                if (img.gameObject.name == "Icon" && animal.idleSprite != null)
                    img.sprite = animal.idleSprite; // 🐾 Show animal icon
            }
        }
    }

}