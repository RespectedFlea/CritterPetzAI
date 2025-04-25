using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CritterPetz;

public class RoomEggSelector : MonoBehaviour
{
    public static RoomEggSelector Instance;

    [Header("UI References")]
    public GameObject panel;
    public Transform eggGrid;
    public GameObject eggButtonPrefab;
    public Button addButton;
    public Button cancelButton;

    private int selectedSlotIndex = -1;
    private EggData selectedEgg;
    private GameObject selectedButton;
    private bool slotOccupiedOnOpen = false;

    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this.gameObject);
        Instance = this;

        panel.SetActive(false);

        addButton.onClick.AddListener(OnAddOrRemoveButtonClicked);
        cancelButton.onClick.AddListener(ClosePanel);
        addButton.interactable = false;
    }

    public void OpenPanelByIndex(int index)
    {
        if (RoomManager.Instance.IsSlotReadyToHatch(index))
        {
            Debug.Log($"[RoomEggSelector] Slot {index} is ready to hatch — selector disabled.");
            return;
        }

        selectedSlotIndex = index;
        selectedEgg = null;
        selectedButton = null;
        slotOccupiedOnOpen = !RoomManager.Instance.IsSlotEmpty(index);

        panel.SetActive(true);
        PopulateEggButtons();
        UpdateAddButtonState();
    }

    void PopulateEggButtons()
    {
        foreach (Transform child in eggGrid)
        {
            Destroy(child.gameObject);
        }

        var grouped = new Dictionary<EggData, int>();
        foreach (var egg in InventoryManager.Instance.GetAllEggs())
        {
            if (grouped.ContainsKey(egg))
                grouped[egg]++;
            else
                grouped[egg] = 1;
        }

        foreach (var pair in grouped)
        {
            var egg = pair.Key;
            var count = pair.Value;

            var btn = Instantiate(eggButtonPrefab, eggGrid);
            var text = btn.GetComponentInChildren<TextMeshProUGUI>();
            if (text != null)
                text.text = $"{egg.eggName} x{count}";

            var imgs = btn.GetComponentsInChildren<Image>();
            foreach (var img in imgs)
            {
                if (img.gameObject.name == "Icon")
                    img.sprite = egg.eggSprite;
            }

            if (btn.TryGetComponent<EggButtonUI>(out var buttonLogic))
            {
                buttonLogic.SetData(egg);
            }
        }
    }

    public void OnEggClicked(EggData egg, GameObject buttonObject)
    {
        selectedEgg = egg;
        selectedButton = buttonObject;

        HighlightButton(buttonObject);
        UpdateAddButtonState();
    }

    void HighlightButton(GameObject btn)
    {
        foreach (Transform child in eggGrid)
        {
            if (child.TryGetComponent<Image>(out var image))
            {
                image.color = (child.gameObject == btn) ? Color.yellow : Color.white;
            }
        }
    }


    void UpdateAddButtonState()
    {
        var label = addButton.GetComponentInChildren<TextMeshProUGUI>();

        if (selectedEgg != null)
        {
            addButton.interactable = true;
            label.text = "Add";
        }
        else if (slotOccupiedOnOpen)
        {
            addButton.interactable = true;
            label.text = "Remove";
        }
        else
        {
            addButton.interactable = false;
            label.text = "Add";
        }
    }

    void OnAddOrRemoveButtonClicked()
    {
        if (selectedSlotIndex == -1) return;

        if (selectedEgg != null)
        {
            // Adding new egg
            InventoryManager.Instance.RemoveEgg(selectedEgg);
            RoomManager.Instance.SetEquippedEgg(selectedSlotIndex, selectedEgg);
        }
        else if (slotOccupiedOnOpen)
        {
            // Removing existing egg
            RoomManager.Instance.ClearSlot(selectedSlotIndex, true);

        }

        ClosePanel();
    }

    void ClosePanel()
    {
        panel.SetActive(false);
        selectedSlotIndex = -1;
        selectedEgg = null;
        selectedButton = null;
        slotOccupiedOnOpen = false;
    }
}
