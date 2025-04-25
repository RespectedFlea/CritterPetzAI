using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CritterPetz;

public class RoomManager : MonoBehaviour
{
    public static RoomManager Instance;

    [Header("UI Slot Assignments")]
    public EggData[] equippedEggs = new EggData[3];
    public Image[] eggSlotIcons;
    public TextMeshProUGUI[] hatchTimerTexts;

    [Header("Hatch Settings")]
    public float hatchDuration = 30f;
    private float[] hatchTimers = new float[3];
    private bool[] isHatching = new bool[3];
    private bool[] readyToHatch = new bool[3];

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    void Update()
    {
        for (int i = 0; i < hatchTimers.Length; i++)
        {
            if (isHatching[i])
            {
                hatchTimers[i] -= Time.deltaTime;

                if (hatchTimers[i] <= 0)
                {
                    hatchTimers[i] = 0;
                    isHatching[i] = false;
                    readyToHatch[i] = true;

                    Debug.Log($"🐣 Egg in slot {i} is ready to hatch!");

                    if (hatchTimerTexts[i] != null)
                        hatchTimerTexts[i].text = "Hatch!";
                }
                else
                {
                    if (hatchTimerTexts[i] != null)
                        hatchTimerTexts[i].text = $"{Mathf.CeilToInt(hatchTimers[i])}s";
                }
            }
            else
            {
                if (!readyToHatch[i])
                {
                    if (hatchTimerTexts[i] != null && hatchTimerTexts[i].text != "Hatch!")
                        hatchTimerTexts[i].text = "";
                }
            }
        }
    }

    public EggData GetEquippedEgg(int index)
    {
        if (index < 0 || index >= equippedEggs.Length) return null;
        return equippedEggs[index];
    }

    public void SetEquippedEgg(int index, EggData egg)
    {
        if (index < 0 || index >= equippedEggs.Length) return;
        equippedEggs[index] = egg;
    }

    public void UpdateSlotIcon(int index, Sprite icon)
    {
        if (index < 0 || index >= eggSlotIcons.Length) return;

        eggSlotIcons[index].sprite = icon;
        eggSlotIcons[index].color = Color.white; // Show icon
    }

    public void StartHatchTimer(int index)
    {
        if (index < 0 || index >= hatchTimers.Length) return;

        hatchTimers[index] = hatchDuration;
        isHatching[index] = true;
        readyToHatch[index] = false;

        if (hatchTimerTexts[index] != null)
            hatchTimerTexts[index].text = $"{Mathf.CeilToInt(hatchDuration)}s";
    }

    public void AttemptHatch(int index)
    {
        if (index < 0 || index >= readyToHatch.Length) return;

        if (readyToHatch[index])
        {
            Debug.Log($"🌟 Starting hatch animation for slot {index}");

            readyToHatch[index] = false;

            // TODO: Play hatch animation here!

            // After animation, spawn the infant pet
            SpawnInfantAnimal(index);
        }
        else
        {
            Debug.Log($"Slot {index} not ready to hatch yet.");
        }
    }

    private void SpawnInfantAnimal(int index)
    {
        Debug.Log($"🐾 Infant animal spawned in slot {index}!");
        // TODO: Actually instantiate a pet prefab here
    }
}
