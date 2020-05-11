using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinDoor : Door
{
    public string pin;
    public string entry;
    public GameObject incorrectSoundPref;
    public GameObject correctSoundPref;

    public override void Interact() {
        if(pin == entry) {
            isLocked = false;
            Instantiate(correctSoundPref, transform.position, Quaternion.identity);
        } else {
            Instantiate(incorrectSoundPref, transform.position, Quaternion.identity);
        }
    }

    public void EnterCharacter(string character) {
        entry += character;
        entry = entry.Substring(Mathf.Max(entry.Length - pin.Length, 0));
        GetComponentInChildren<TMPro.TextMeshPro>().text = entry;
    }
}
