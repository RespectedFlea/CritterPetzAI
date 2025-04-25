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

    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this.gameObject);
        Instance = this;

        panel.SetActive(false);

        addButton.onClick.AddListener(OnAddButtonClicked);
        cancelButton.onClick.AddListener(ClosePanel);
        addButton.interactable = false;
    }

    public void OpenPanelByIndex(int index)
    {
        selectedSlotIndex = index;
        selectedEgg = null;
        selectedButton = null;

        panel.SetActive(true);
        PopulateEggButtons();

        var hasEgg = RoomManager.Instance.GetEquippedEgg(index) != null;
        UpdateAddButtonLabel(hasEgg);
    }

    void PopulateEggButtons()
    {
        foreach (Transform child in eggGrid)
            Destroy(child.gameObject);

        Dictionary<EggData, int> grouped = new Dictionary<EggData, int>();
        foreach (EggData egg in InventoryManager.Instance.GetAllEggs())
        {
            if (grouped.ContainsKey(egg)) grouped[egg]++;
            else grouped[egg] = 1;
        }

        foreach (var pair in grouped)
        {
            EggData egg = pair.Key;
            int count = pair.Value;

            GameObject btn = Instantiate(eggButtonPrefab, eggGrid);

            var text = btn.GetComponentInChildren<TextMeshProUGUI>();
            if (text != null) text.text = $"{egg.eggName} x{count}";

            var imgs = btn.GetComponentsInChildren<Image>();
            foreach (var img in imgs)
                if (img.gameObject.name == "Icon")
                    img.sprite = egg.eggSprite;

            // Set egg data and manually wire up OnClick
            EggButtonUI buttonLogic = btn.GetComponent<EggButtonUI>();
            if (buttonLogic != null)
            {
                buttonLogic.SetData(egg);
                btn.GetComponent<Button>().onClick.AddListener(buttonLogic.OnClick);
            }
        }

        addButton.interactable = false;
    }

    public void OnEggClicked(EggData egg, GameObject buttonObject)
    {
        selectedEgg = egg;
        selectedButton = buttonObject;

        HighlightButton(buttonObject);

        var hasEgg = RoomManager.Instance.GetEquippedEgg(selectedSlotIndex) != null;
        UpdateAddButtonLabel(hasEgg);

        Debug.Log($"[RoomEggSelector] Selected egg: {egg.eggName}");
    }

    void HighlightButton(GameObject btn)
    {
        foreach (Transform child in eggGrid)
        {
            var image = child.GetComponent<Image>();
            if (image != null)
                image.color = (child.gameObject == btn) ? Color.yellow : Color.white;
        }
    }

    void UpdateAddButtonLabel(bool hasEgg)
    {
        addButton.interactable = selectedEgg != null;

        string label = hasEgg ? "Swap" : "Add";
        var labelText = addButton.GetComponentInChildren<TextMeshProUGUI>();
        if (labelText != null)
        {
            labelText.text = label;
        }
    }

    void OnAddButtonClicked()
    {
        if (selectedEgg == null || selectedSlotIndex == -1) return;

        var existing = RoomManager.Instance.GetEquippedEgg(selectedSlotIndex);
        if (existing != null)
        {
            InventoryManager.Instance.AddEgg(existing);
        }

        InventoryManager.Instance.RemoveEgg(selectedEgg);
        RoomManager.Instance.SetEquippedEgg(selectedSlotIndex, selectedEgg);
        RoomManager.Instance.UpdateSlotIcon(selectedSlotIndex, selectedEgg.eggSprite);
        RoomManager.Instance.StartHatchTimer(selectedSlotIndex);

        ClosePanel();
    }

    void ClosePanel()
    {
        panel.SetActive(false);
        selectedSlotIndex = -1;
        selectedEgg = null;
        selectedButton = null;
    }
}
