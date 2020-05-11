using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayButton : MonoBehaviour
{

    public void StartGame() {
        SceneManager.LoadScene(1, LoadSceneMode.Single);
    }

    public void DanceMode() {
        SceneManager.LoadScene(3, LoadSceneMode.Single);
    }
}
