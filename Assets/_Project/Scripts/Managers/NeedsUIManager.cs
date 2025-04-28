using UnityEngine;

public class NeedsUIManager : MonoBehaviour
{
    [Header("Testing Controls")]
    [Range(0.1f, 100f)]
    public float sleepDrainTimeMultiplier = 1f; // Adjustable in Inspector

    public static NeedsUIManager Instance;

    [Header("Needs References")]
    public NeedBarUI sleepBar;

    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
    }

    /// <summary>
    /// Updates the sleep bar value (0-100%).
    /// </summary>
    public void UpdateSleepNeed(float sleepValue)
    {
        if (sleepBar != null)
            sleepBar.SetValue(sleepValue);
    }

    // 🚀 Later you can add more methods for Hunger, Fun, etc like:
    // public void UpdateHungerNeed(float hungerValue) { ... }
}
