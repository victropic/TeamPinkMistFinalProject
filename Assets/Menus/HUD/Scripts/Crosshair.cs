using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crosshair : MonoBehaviour
{

    UnityEngine.UI.Image image;
    Player player;
    
    void Start()
    {
        image = GetComponent<UnityEngine.UI.Image>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    void Update()
    {
        image.color = Color.Lerp(Color.white, Color.red, player.GetWeaponCharge());
        
    }
}
