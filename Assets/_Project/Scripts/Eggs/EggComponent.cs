using UnityEngine;
using CritterPetz;

public class EggComponent : MonoBehaviour
{
    [Header("Egg Data")]
    public EggData eggData;

    public void Initialize(EggData data)
    {
        eggData = data;

        // Update sprite visually
        var spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null && data.eggSprite != null)
        {
            spriteRenderer.sprite = data.eggSprite;
        }
    }

    // Optional: You could hook this up to user interactions later
    private void OnMouseDown()
    {
        Debug.Log($"Clicked on egg: {eggData.eggName}");
        // Later: Open egg menu or allow swap/remove
    }
}
