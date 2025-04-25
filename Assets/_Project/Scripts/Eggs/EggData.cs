using UnityEngine;

namespace CritterPetz
{
    [CreateAssetMenu(fileName = "NewEgg", menuName = "CritterPetz/Egg")]
    public class EggData : ScriptableObject
    {
        public string eggName;
        public Sprite eggSprite;
        public float hatchDuration = 30f;
    }
}
