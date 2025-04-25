using UnityEngine;
using UnityEngine.UI;

public class DebugDropdownController : MonoBehaviour
{
    [Header("UI References")]
    public GameObject debugDropdownPanel;
    public GameObject addEggPanel;

    public Button toggleDebugButton;
    public Button openAddEggButton;

    void Start()
    {
        if (toggleDebugButton != null)
            toggleDebugButton.onClick.AddListener(ToggleDropdown);
        else
            Debug.LogWarning("[DebugDropdownController] Toggle Debug Button not assigned.");

        if (openAddEggButton != null)
            openAddEggButton.onClick.AddListener(ToggleAddEggPanel);
        else
            Debug.LogWarning("[DebugDropdownController] Open Add Egg Button not assigned.");

        if (debugDropdownPanel != null)
            debugDropdownPanel.SetActive(false);

        if (addEggPanel != null)
            addEggPanel.SetActive(false);
    }

    public void ToggleDropdown()
    {
        if (debugDropdownPanel != null)
        {
            debugDropdownPanel.SetActive(!debugDropdownPanel.activeSelf);
        }
        else
        {
            Debug.LogError("[DebugDropdownController] debugDropdownPanel not assigned.");
        }
    }

    public void ToggleAddEggPanel()
    {
        if (addEggPanel != null)
        {
            addEggPanel.SetActive(!addEggPanel.activeSelf);
        }
        else
        {
            Debug.LogError("[DebugDropdownController] addEggPanel not assigned.");
        }
    }
}
