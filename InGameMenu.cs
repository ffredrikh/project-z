using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InGameMenu : MonoBehaviour
{
    public GameObject InGameMenuObj;
    public GameObject InGameMenuIcon;

    private const string MENU = "Menu";

    public void OpenInGameMenu()
    {
        Time.timeScale = 0;
        InGameMenuIcon.SetActive(false);
        InGameMenuObj.SetActive(true);
    }

    public void Return()
    {
        InGameMenuObj.SetActive(false);
        InGameMenuIcon.SetActive(true);
        Time.timeScale = 1;
    }

    public void Restart()
    {
        InGameMenuObj.SetActive(false);
        InGameMenuIcon.SetActive(true);
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene(MENU);
    }

}
