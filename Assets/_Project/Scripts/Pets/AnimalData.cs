using UnityEngine;

[CreateAssetMenu(fileName = "NewAnimalData", menuName = "CritterPetz/Animal Data", order = 1)]
public class AnimalData : ScriptableObject
{
    public string animalName;
    public string animalType;

    [Header("Brain Settings")]
    public float curiosityLevel = 0.5f;
    public float playfulness = 0.6f;
    public float intelligence = 0.5f;

    [Header("Movement Settings")]
    public float walkSpeed = 1.5f;
    public float maxWalkSpeed = 2.5f; // 🆕 New field you add here

    [Header("Hunger Settings")]
    public float hungerRate = 0.1f;

    [Header("Sprites")]
    public Sprite idleSprite;
    public Sprite walkSprite;
    public Sprite sleepSprite;

    [Header("Sleep Settings")]
    public float hoursToFullSleep = 8f;
    public float sleepDesireThreshold = 30f;
    public float napFillAmount = 25f;
    public float forcedAwakeTimeMinutes = 15f;
    public float bedSleepBonusMultiplier = 1.2f;
    public float wakeUpRefillBonus = 5f; // (percent bonus when waking early)
    public float fatigueThreshold = 20f; // (below this % pet moves slower)
    public float fatigueSpeedMultiplier = 0.5f; // (move at half speed when exhausted)

}
