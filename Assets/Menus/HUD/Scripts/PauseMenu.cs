using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public void GoToMainMenu() {
        SceneManager.LoadScene(0, LoadSceneMode.Single);
    }

    public void Quit() {
        Application.Quit();
    }
}
