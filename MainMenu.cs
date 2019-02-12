using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

    private const string MENU = "Menu";
    private const string INTRO = "Intro";
    private const string LEVEL1 = "Level 1";

    public Animator MenuAnimations;
    public GameObject Menu;
    public GameObject PlayMenu;

    /********** MAIN MENU **********/

    public void Play()
    {
        Menu.SetActive(false);
        MenuAnimations.Play("PlayToPlayMenu");
        PlayMenu.SetActive(true);
    }

    public void Options()
    {
        SceneManager.LoadScene("Options");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    /********** PLAY MENU **********/

    public void PlayMenuBack()
    {
        PlayMenu.SetActive(false);
        Menu.SetActive(true);
    }


    public void Intro()
    {
        SceneManager.LoadScene(INTRO);
    }


    public void Level1()
    {
        SceneManager.LoadScene(LEVEL1);
    }

    
}
