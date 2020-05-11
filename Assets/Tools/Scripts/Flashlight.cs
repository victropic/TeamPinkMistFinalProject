using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flashlight : Tool
{
    Light spotLight;
    AudioSource buttonPress;

    void Start()
    {
        spotLight = GetComponentInChildren<Light>();
        buttonPress = GetComponent<AudioSource>();
    }

    public override void Use() {
        spotLight.enabled = !spotLight.enabled;
        buttonPress.Play();
    }
}
