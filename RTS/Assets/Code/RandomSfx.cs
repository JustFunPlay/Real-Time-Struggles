using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSfx : MonoBehaviour
{
    public AudioSource[] sfxs;

    public void PlaySFX()
    {
        int i = Random.Range(0, sfxs.Length - 1);
        sfxs[i].pitch = Random.Range(0.5f, 1.5f);
        sfxs[i].Play();
    }
}
