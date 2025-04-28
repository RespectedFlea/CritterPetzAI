using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using CritterPetz;
using TMPro;

public class AddEggPanelController : MonoBehaviour
{
    [Header("UI References")]
    public GameObject buttonPrefab;             // Prefab for each egg button
    public Transform buttonContainer;           // Where to place the egg buttons

    [Header("Egg Data")]
    public List<EggData> availableEggs;         // Assign these in Inspector

    void OnEnable()
    {
        PopulateButtons();
    }

    void PopulateButtons()
    {
        if (buttonPrefab == null)
        {
            Debug.LogError("[AddEggPanel] buttonPrefab is not assigned!");
            return;
        }

        if (buttonContainer == null)
        {
            Debug.LogError("[AddEggPanel] buttonContainer is not assigned!");
            return;
        }

        if (availableEggs == null || availableEggs.Count == 0)
        {
            Debug.LogWarning("[AddEggPanel] availableEggs list is empty or not assigned.");
            return;
        }

        // Clear existing buttons first
        foreach (Transform child in buttonContainer)
        {
            Destroy(child.gameObject);
        }

        // Create a button for each EggData
        foreach (EggData egg in availableEggs)
        {
            GameObject newButton = Instantiate(buttonPrefab, buttonContainer);

            // Set egg name
            var textComponent = newButton.GetComponentInChildren<TextMeshProUGUI>();
            if (textComponent != null)
                textComponent.text = egg.eggName;

            // Set icon sprite
            var imageComponents = newButton.GetComponentsInChildren<Image>();
            foreach (var image in imageComponents)
            {
                if (image.gameObject.name == "Icon")
                {
                    image.sprite = egg.eggSprite;
                }
            }

            // Add click action
            newButton.GetComponent<Button>().onClick.AddListener(() =>
            {
                InventoryManager.Instance.AddEgg(egg);
                Debug.Log($"[AddEggPanel] Added egg: {egg.eggName}");
            });
        }

    }
}