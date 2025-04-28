using UnityEngine;

[CreateAssetMenu(fileName = "NewAnimalData", menuName = "CritterPetz/Animal Data", order = 1)]
public class AnimalData : ScriptableObject
{
    public string animalName;
    public string animalType;

    [Header("Movement Settings")]
    public float walkSpeed = 1.5f;
    public float maxWalkSpeed = 2.5f; // 🆕 New field you add here
    public float sleepDuration = 5f;
    public float hungerRate = 0.1f;
    public float curiosityLevel = 0.5f;
    public float playfulness = 0.6f;
    public float intelligence = 0.5f;

    [Header("Sprites")]
    public Sprite idleSprite;
    public Sprite walkSprite;
    public Sprite sleepSprite;

    [Header("Sleep Settings")]
    public float hoursToFullSleep = 8f;    // How many real-world hours it takes to refill sleep from 0 ➔ 100%
    public float sleepDesireThreshold = 30f; // Below this %, animal starts wanting to nap
    public float napFillAmount = 25f;       // Nap fills this much % (before it wakes itself)
    public float forcedAwakeTimeMinutes = 15f; // How long after player wakes it up, it must stay awake
    public float bedSleepBonusMultiplier = 1.2f; // Beds give faster/better sleep (future feature)
}
