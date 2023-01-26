using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DifficultyLoader : MonoBehaviour
{
    public static DifficultyLoader instance;

    public Difficulty difficulty;
    public int startingCash;
    public Army playerArmy;

    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
}

[System.Serializable]
public enum Difficulty
{
    Easy,
    Normal,
    Hard
}