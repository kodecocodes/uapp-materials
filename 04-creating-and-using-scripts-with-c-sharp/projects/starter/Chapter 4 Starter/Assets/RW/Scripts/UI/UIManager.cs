using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Text ammunitionAmountText;
    public Slider ammuntionReadySlider;

    public void InitialSetup(float maximumTimerValue)
    {
        ammunitionAmountText.text = "0";
        ammuntionReadySlider.maxValue = maximumTimerValue;
    }

    public void UpdateAmmunitionText(int value)
    {
        ammunitionAmountText.text = value.ToString();
    }

    public void UpdateAmmunitionTimerValue(float value)
    {
        ammuntionReadySlider.value = value;
    }
}