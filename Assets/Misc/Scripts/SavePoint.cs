using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavePoint : Interactable
{
    GameController gameController;

    void Start()
    {
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
    }

    public override void Interact() {
        gameController.SaveGame();
    }
}
