using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerSource : Interactable
{
    public bool isOn = true;

    public override void Interact() {
        isOn = !isOn;
    }
}
