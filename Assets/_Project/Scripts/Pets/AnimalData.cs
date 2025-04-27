using UnityEngine;

[CreateAssetMenu(fileName = "NewAnimalData", menuName = "CritterPetz/Animal Data", order = 1)]
public class AnimalData : ScriptableObject
{
    public string animalName;
    public string animalType;

    [Header("Movement Settings")]
    public float walkSpeed = 1.5f;
    public float sleepDuration = 5f;
    public float hungerRate = 0.1f;
    public float curiosityLevel = 0.5f;
    public float playfulness = 0.6f;
    public float intelligence = 0.5f;

    [Header("Sprites")]
    public Sprite idleSprite;
    public Sprite walkSprite;
    public Sprite sleepSprite;
}
