using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NeedBarUI : MonoBehaviour
{
    [Header("References")]
    public Image iconImage; // The icon (🛌, 🍔 etc)
    public Slider progressBar; // The bar that fills
    public TMP_Text percentageText; // Text showing 75%

    /// <summary>
    /// Sets the icon sprite for this NeedBar.
    /// </summary>
    public void SetIcon(Sprite newIcon)
    {
        if (iconImage != null)
            iconImage.sprite = newIcon;
    }

    /// <summary>
    /// Updates the progress bar and percentage display.
    /// Value should be between 0-100.
    /// </summary>
    public void SetValue(float value)
    {
        if (progressBar != null)
            progressBar.value = value / 100f; // Slider expects 0-1, not 0-100

        if (percentageText != null)
            percentageText.text = $"{Mathf.RoundToInt(value)}%";
    }
}
