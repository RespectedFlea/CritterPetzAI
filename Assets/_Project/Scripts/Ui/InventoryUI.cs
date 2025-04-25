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

        if (buttonPrefab == null)
        {
            Debug.LogError("[InventoryUI] buttonPrefab is NOT assigned!");
            return;
        }

        if (buttonContainer == null)
        {
            Debug.LogError("[InventoryUI] buttonContainer is NOT assigned!");
            return;
        }

        if (inventoryPanel == null)
        {
            Debug.LogError("[InventoryUI] inventoryPanel is NOT assigned!");
            return;
        }

        // Clear old buttons
        foreach (Transform child in buttonContainer)
        {
            Destroy(child.gameObject);
        }

        // Get eggs
        List<EggData> eggs = InventoryManager.Instance.GetAllEggs();

        // Generate buttons
        foreach (EggData egg in eggs)
        {
            GameObject newButton = Instantiate(buttonPrefab, buttonContainer);

            var text = newButton.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            if (text != null)
                text.text = egg.eggName;

            var images = newButton.GetComponentsInChildren<Image>();
            foreach (Image img in images)
            {
                if (img.gameObject.name == "Icon")
                    img.sprite = egg.eggSprite;
            }
        }
    }
}
