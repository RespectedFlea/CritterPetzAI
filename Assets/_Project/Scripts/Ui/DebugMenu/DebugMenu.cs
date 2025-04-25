using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using CritterPetz;

public class DebugMenu : MonoBehaviour
{
    public GameObject buttonPrefab;
    public Transform buttonContainer;
    public List<EggData> allEggs;
    public GameObject addEggPanel;

    void Start()
    {
        GenerateFeatureButtons();
    }

    void GenerateFeatureButtons()
    {
        GameObject eggMenuButton = Instantiate(buttonPrefab, buttonContainer);
        eggMenuButton.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "Open Add Egg Menu";

        eggMenuButton.GetComponent<Button>().onClick.AddListener(() =>
        {
            addEggPanel.SetActive(!addEggPanel.activeSelf);
        });
    }
}
