using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEnd : MonoBehaviour
{
    public static GameEnd instance;
    public GameObject victoryScreen;
    public GameObject loserScreen;
    public GameObject[] others;

    private void Start()
    {
        instance = this;
    }
    public void CheckGameEnd()
    {
        bool playerDefeat = false;
        foreach (HQBuilding hq in PlayerTroopManager.instance.HQs)
        {
            if (hq.army == PlayerCam.playerArmy)
                playerDefeat = true;
        }
        if (playerDefeat)
        {
            StartCoroutine(GetEnding(false));
        }
        else if (PlayerTroopManager.instance.HQs.Count == 1)
        {
            StartCoroutine(GetEnding(true));
        }
    }

    IEnumerator GetEnding(bool win)
    {
        for (int i = 0; i < others.Length; i++)
        {
            others[i].SetActive(false);
        }
        for (int i = 0; i < 10; i++)
        {
            Time.timeScale = 1 - 0.1f*(i +1 );
            if (i == 9)
                Time.timeScale = 0.000000001f;
            Time.fixedDeltaTime = Time.timeScale * 0.02f;
            yield return new WaitForSecondsRealtime(0.1f);
        }
        if (win)
            victoryScreen.SetActive(true);
        else
            loserScreen.SetActive(true);
    }
}
