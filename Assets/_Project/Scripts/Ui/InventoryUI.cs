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

        // Simplify 'new' expression for Dictionary initialization
        Dictionary<EggData, int> eggCounts = new();
        foreach (EggData egg in InventoryManager.Instance.GetAllEggs())
        {
            if (eggCounts.ContainsKey(egg))
                eggCounts[egg]++;
            else
                eggCounts[egg] = 1;
        }

        // Display each group with count
        foreach (KeyValuePair<EggData, int> pair in eggCounts)
        {
            EggData egg = pair.Key;
            int count = pair.Value;

            GameObject newButton = Instantiate(buttonPrefab, buttonContainer);

            // Update name with count
            var text = newButton.GetComponentInChildren<TextMeshProUGUI>();
            if (text != null)
                text.text = $"{egg.eggName} x{count}";

            // Update icon
            var images = newButton.GetComponentsInChildren<Image>();
            foreach (Image img in images)
            {
                if (img.gameObject.name == "Icon")
                    img.sprite = egg.eggSprite;
            }
        }
    }
}