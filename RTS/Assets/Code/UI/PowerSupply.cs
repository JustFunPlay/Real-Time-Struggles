using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerSupply : MonoBehaviour
{
    public Slider slider;
    public Text text;
    public Image sliderImage;
    void FixedUpdate()
    {
        int maxPower = 0;
        int currentPower = 0;
        foreach (HQBuilding hq in PlayerTroopManager.instance.HQs)
        {
            if (hq.army == PlayerCam.playerArmy)
            {
                maxPower = hq.maxPower;
                currentPower = hq.currentPower;
            }
        }

        text.text = $"{maxPower}/{currentPower}";
        
        slider.maxValue = maxPower;
        
        if (currentPower >= maxPower)
        {
            slider.value = maxPower;
            sliderImage.color = Color.red;
        }
        else
        {
            slider.value = currentPower;
            if (maxPower - currentPower >= 5)
                sliderImage.color = Color.yellow;
            else
                sliderImage.color = Color.cyan;
        }
    }
}
