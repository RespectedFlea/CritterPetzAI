using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CritterPetz;

/// <summary>
/// Manages the egg and pet selection panel UI for placing or removing into room slots.
/// </summary>
public class RoomEggSelector : MonoBehaviour
{
    public static RoomEggSelector Instance;

    [Header("UI References")]
    public GameObject panel;
    public Transform eggGrid;
    public Transform petGrid;
    public GameObject eggButtonPrefab;
    public GameObject petButtonPrefab;
    public Button addButton;
    public Button cancelButton;
    public Button eggTabButton;
    public Button petTabButton;

    private int selectedSlotIndex = -1;
    private EggData selectedEgg = null;
    private AnimalData selectedAnimal = null;
    private GameObject selectedButton = null;
    private bool slotOccupiedOnOpen = false;

    private enum TabType { Egg, Pet }
    private TabType currentTab = TabType.Egg;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;

        panel.SetActive(false);

        addButton.onClick.AddListener(OnAddOrRemoveButtonClicked);
        cancelButton.onClick.AddListener(ClosePanel);
        eggTabButton.onClick.AddListener(OnEggTabClicked);
        petTabButton.onClick.AddListener(OnPetTabClicked);

        addButton.interactable = false;
    }

    public void OpenPanelByIndex(int index)
    {
        selectedSlotIndex = index;
        selectedEgg = null;
        selectedAnimal = null;
        selectedButton = null;
        slotOccupiedOnOpen = !RoomManager.Instance.IsSlotEmpty(index);

        panel.SetActive(true);

        // Always open with Egg tab
        currentTab = TabType.Egg;
        eggGrid.gameObject.SetActive(true);
        petGrid.gameObject.SetActive(false);

        RefreshCurrentTab();
        UpdateAddButtonState();
    }

    public void OnEggTabClicked()
    {
        currentTab = TabType.Egg;
        eggGrid.gameObject.SetActive(true);
        petGrid.gameObject.SetActive(false);
        RefreshCurrentTab();
    }

    public void OnPetTabClicked()
    {
        currentTab = TabType.Pet;
        petGrid.gameObject.SetActive(true);
        eggGrid.gameObject.SetActive(false);
        RefreshCurrentTab();
    }

    private void RefreshCurrentTab()
    {
        ClearGrids();

        if (currentTab == TabType.Egg)
            PopulateEggButtons();
        else if (currentTab == TabType.Pet)
            PopulatePetButtons();
    }

    private void ClearGrids()
    {
        foreach (Transform child in eggGrid)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in petGrid)
        {
            Destroy(child.gameObject);
        }
    }

    private void PopulateEggButtons()
    {
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
            int count = pair.Value;

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

    private void PopulatePetButtons()
    {
        var grouped = new Dictionary<AnimalData, int>();
        foreach (var animal in InventoryManager.Instance.GetAllAnimals())
        {
            if (grouped.ContainsKey(animal))
                grouped[animal]++;
            else
                grouped[animal] = 1;
        }

        foreach (var pair in grouped)
        {
            var animal = pair.Key;
            int count = pair.Value;

            var btn = Instantiate(petButtonPrefab, petGrid);
            var text = btn.GetComponentInChildren<TextMeshProUGUI>();
            if (text != null)
                text.text = $"{animal.animalName} x{count}";

            var imgs = btn.GetComponentsInChildren<Image>();
            foreach (var img in imgs)
            {
                if (img.gameObject.name == "Icon")
                    img.sprite = animal.idleSprite;
            }

            if (btn.TryGetComponent<PetButtonUI>(out var buttonLogic))
            {
                buttonLogic.SetData(animal);
            }
        }
    }

    public void OnEggClicked(EggData egg, GameObject buttonObject)
    {
        selectedEgg = egg;
        selectedAnimal = null;
        selectedButton = buttonObject;
        HighlightButton(buttonObject);
        UpdateAddButtonState();
    }

    public void OnPetClicked(AnimalData animal, GameObject buttonObject)
    {
        selectedAnimal = animal;
        selectedEgg = null;
        selectedButton = buttonObject;
        HighlightButton(buttonObject);
        UpdateAddButtonState();
    }

    private void HighlightButton(GameObject btn)
    {
        Transform activeGrid = (currentTab == TabType.Egg) ? eggGrid : petGrid;

        foreach (Transform child in activeGrid)
        {
            if (child.TryGetComponent<Image>(out var image))
            {
                image.color = (child.gameObject == btn) ? Color.yellow : Color.white;
            }
        }
    }

    private void UpdateAddButtonState()
    {
        var label = addButton.GetComponentInChildren<TextMeshProUGUI>();

        if (selectedEgg != null || selectedAnimal != null)
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

    private void OnAddOrRemoveButtonClicked()
    {
        if (selectedSlotIndex == -1) return;

        if (selectedEgg != null)
        {
            InventoryManager.Instance.RemoveEgg(selectedEgg);
            RoomManager.Instance.SetEquippedEgg(selectedSlotIndex, selectedEgg);
        }
        else if (selectedAnimal != null)
        {
            InventoryManager.Instance.RemoveAnimal(selectedAnimal);
            RoomManager.Instance.SetEquippedAnimal(selectedSlotIndex, selectedAnimal);
        }
        else if (slotOccupiedOnOpen)
        {
            RoomManager.Instance.ClearSlot(selectedSlotIndex, true);
        }

        ClosePanel();
    }

    private void ClosePanel()
    {
        panel.SetActive(false);
        selectedSlotIndex = -1;
        selectedEgg = null;
        selectedAnimal = null;
        selectedButton = null;
        slotOccupiedOnOpen = false;
    }
}
