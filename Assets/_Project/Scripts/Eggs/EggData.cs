using UnityEngine;

namespace CritterPetz
{
    [CreateAssetMenu(fileName = "NewEggData", menuName = "CritterPetz/Egg Data")]
    public class EggData : ScriptableObject
    {
        public string eggName;
        public EggType eggType;
        public Sprite eggSprite;
        [TextArea]
        public string description;
    }
}
