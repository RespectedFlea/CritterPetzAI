using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // --------------------------
    // Save/Load Currency
    // --------------------------
    public void SaveCurrency(int coins)
    {
        PlayerPrefs.SetInt("CritterCoins", coins);
        PlayerPrefs.Save();
    }

    public int LoadCurrency()
    {
        return PlayerPrefs.GetInt("CritterCoins", 0); // default to 0 if not saved
    }

    // --------------------------
    // Save/Load Egg Slot
    // --------------------------
    public void SaveEggSlot(int slotIndex, string eggType)
    {
        PlayerPrefs.SetString("EggSlot_" + slotIndex, eggType);
        PlayerPrefs.Save();
    }

    public string LoadEggSlot(int slotIndex)
    {
        return PlayerPrefs.GetString("EggSlot_" + slotIndex, "");
    }

    // --------------------------
    // Clear all saved data
    // --------------------------
    public void ClearAllData()
    {
        PlayerPrefs.DeleteAll();
    }
}
