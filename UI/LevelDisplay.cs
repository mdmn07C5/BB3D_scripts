using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelDisplay : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField]
    private TextMeshProUGUI currentLevelDisplay;
    [SerializeField]
    private TextMeshProUGUI nextLevelDisplay;

    [SerializeField]
    private Image slider;

    [SerializeField]
    private IntReference currentLevel;

    [SerializeField]
    private IntReference currentStars;

    [SerializeField]
    private IntReference maxStars;

    private void Start()
    {
        ShowLevelDisplay();
    }

    public void ShowLevelDisplay()
    {
        currentLevelDisplay.text = currentLevel.Value + "";
        nextLevelDisplay.text = (currentLevel.Value + 1) + "";
    }

    public void ResetSlider()
    {
        slider.fillAmount = 0;
    }

    public void UpdateSlider()
    {
        float sliderValue = (float)currentStars.Value / (float)maxStars.Value;
        slider.fillAmount = sliderValue;
    }

}
