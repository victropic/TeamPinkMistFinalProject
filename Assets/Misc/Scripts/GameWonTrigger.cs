using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameWonTrigger : MonoBehaviour
{
    void OnTriggerEnter(Collider coll) {
        if(coll.tag == "Player") {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            SceneManager.LoadScene(2);
        }
    }
}
