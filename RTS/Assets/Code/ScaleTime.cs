using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleTime : MonoBehaviour
{
    float normalTime;
    private void Start()
    {
        normalTime = Time.timeScale;
    }

    public void NormalTime()
    {
        Time.timeScale = normalTime;
        Time.fixedDeltaTime = Time.timeScale * 0.02f;
    }

    public void DoubleTime()
    {
        Time.timeScale = 2 * normalTime;
        Time.fixedDeltaTime = Time.timeScale * 0.02f;
    }
    public void QuadTime()
    {
        Time.timeScale = 4 * normalTime;
        Time.fixedDeltaTime = Time.timeScale * 0.02f;
    }
    public void OcTime()
    {
        Time.timeScale = 8 * normalTime;
        Time.fixedDeltaTime = Time.timeScale * 0.02f;
    }
}
