using UnityEngine;

public class MenuSystem : MonoBehaviour
{
    public GameObject escapeMenu;

    private void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
                escapeMenu.SetActive(true);
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}