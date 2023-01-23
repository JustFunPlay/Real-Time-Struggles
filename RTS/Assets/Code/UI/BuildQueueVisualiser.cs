using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildQueueVisualiser : MonoBehaviour
{
    public Image[] images;
    public TMPro.TextMeshProUGUI text;
    public Color defaultColor;

    private void Start()
    {
        defaultColor = images[0].color;
    }
    void FixedUpdate()
    {
        if (PlayerCam.instance.selectedUnits.Count == 1 && PlayerCam.instance.selectedUnits[0].TryGetComponent<Factory>(out Factory factory) && factory.isBuilding)
        {
            images[0].gameObject.SetActive(true);
            images[0].sprite = factory.troopInProgress.troop.icon;
            images[0].color = Color.gray;
            text.text = factory.timeLeftToBuild.ToString();
            for (int i = 1; i < images.Length; i++)
            {
                if (factory.queue.Count >= i)
                {
                    images[i].sprite = factory.queue[i - 1].troop.icon;
                    images[i].color = Color.white;
                }
                else
                {
                    images[i].sprite = null;
                    images[i].color = defaultColor;
                }
            }
        }
        else
        {
            text.text = null;
            foreach (Image image in images)
            {
                image.sprite = null;
                image.color = default;
            }
        }
    }
}
