using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PinPadButton : Interactable
{

    public PinDoor door;
    string character;
    public GameObject clickSoundPref;

    void Start() {
        character = GetComponentInChildren<TextMeshPro>().text;
    }

    public override void Interact() {
        door.EnterCharacter(character);
        Instantiate(clickSoundPref, transform.position, Quaternion.identity);
    }
}
