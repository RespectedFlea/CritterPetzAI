using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public static RoomManager Instance;

    void Awake()
    {
        // Singleton pattern — one GameManager for the whole game
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    {
        // Initialize game systems here later
    }
}
