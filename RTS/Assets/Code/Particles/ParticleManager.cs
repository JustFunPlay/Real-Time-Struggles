using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    public static ParticleManager instance;

    public LineRenderer example;
    List<LineRenderer> renderers = new List<LineRenderer>();
    int currentLine;
    private void Awake()
    {
        instance = this;
        for (int i = 0; i < 50; i++)
        {
            renderers.Add(Instantiate(example, transform.position, Quaternion.identity, transform));
            renderers[i].gameObject.SetActive(false);
        }
    }

    public void SetLine(Vector3 startpos, Vector3 endpos)
    {
        if (currentLine >= renderers.Count)
            currentLine = 0;
        renderers[currentLine].SetPosition(0, startpos);
        renderers[currentLine].SetPosition(1, endpos);
        renderers[currentLine].gameObject.SetActive(true);
        StartCoroutine(ThrowLine(currentLine));
        currentLine++;

    }

    IEnumerator ThrowLine(int line)
    {
        yield return new WaitForSeconds(0.1f);
        renderers[line].gameObject.SetActive(false);
    }
}
