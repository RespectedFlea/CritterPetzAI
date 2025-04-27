using UnityEngine;

namespace CritterPetz
{
    [CreateAssetMenu(fileName = "NewEgg", menuName = "CritterPetz/Egg")]
    public class EggData : ScriptableObject
    {
        public string eggName;
        public Sprite eggSprite;
        public float hatchDuration = 30f;

        [Header("Physics Settings")]
        public float gravityScale = 1f;
        public float drag = 0f;
        public float angularDrag = 0.05f;
        public float bounciness = 0.1f;

        [Header("Boing Settings")]
        public float squashFactor = 1.2f;
        public float stretchFactor = 0.8f;
        public float boingSpeed = 0.1f;

        [Header("Boing Impact Settings")]
        public float minImpactSpeedForBoing = 1f;
        public float maxImpactSpeed = 10f;
        public float maxSquashMultiplier = 1.5f;

        [Header("Hatch Wiggle Settings")]
        public float wiggleRotationAmount = 10f; // How much tilt (degrees left/right)
        public float wiggleDuration = 0.3f; // How fast one wiggle happens
        public float wiggleInterval = 5f; // How long to wait between wiggles
        public bool wiggleRelativeToCurrentRotation = true; // If true, wiggles relative to whatever rotation the egg has

        [Header("Animal That Hatches")]
        public AnimalData animalData;

    }
}
