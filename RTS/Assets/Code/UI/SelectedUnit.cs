using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectedUnit : MonoBehaviour
{
    public Slider slider;
    public Image sliderImage;
    public Image icon;
    public Sprite multiUnitSprite;
    public TMPro.TextMeshProUGUI text;

    void Update()
    {
        if (PlayerCam.instance.selectedUnits.Count > 1)
        {
            icon.gameObject.SetActive(true);
            int combinedMax = 0;
            int combinedCurrent = 0;
            foreach (UnitBase unit in PlayerCam.instance.selectedUnits)
            {
                combinedMax += unit.maxHP;
                combinedCurrent += unit.currentHP;
            }
            slider.maxValue = combinedMax;
            slider.value = combinedCurrent;
            icon.sprite = multiUnitSprite;
            if (combinedCurrent * 3 <= combinedMax)
                sliderImage.color = Color.red;
            else if (combinedCurrent * 1.5 <= combinedMax)
                sliderImage.color = Color.yellow;
            else
                sliderImage.color = Color.green;
            text.text = "Multiple Selected";
        }
        else if (PlayerCam.instance.selectedUnits.Count == 1)
        {
            icon.gameObject.SetActive(true);
            slider.maxValue = PlayerCam.instance.selectedUnits[0].maxHP;
            slider.value = PlayerCam.instance.selectedUnits[0].currentHP;
            icon.sprite = PlayerCam.instance.selectedUnits[0].icon;
            text.text = PlayerCam.instance.selectedUnits[0].unitName;
            if (PlayerCam.instance.selectedUnits[0].currentHP * 3 <= PlayerCam.instance.selectedUnits[0].maxHP)
                sliderImage.color = Color.red;
            else if (PlayerCam.instance.selectedUnits[0].currentHP * 1.5 <= PlayerCam.instance.selectedUnits[0].maxHP)
                sliderImage.color = Color.yellow;
            else
                sliderImage.color = Color.green;
        }
        else
        {
            slider.value = 0;
            icon.gameObject.SetActive(false) ;
            text.text = null;
        }
    }
}
