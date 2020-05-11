using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnergyBar : MonoBehaviour
{
    public float length;
    RectTransform energyBarRT;
    RectTransform containerRT;
    Image energyBar;
    Image container;
    Player player;

    void Start()
    {
        containerRT = GetComponent<RectTransform>();
        container = GetComponent<Image>(); 
        energyBarRT = transform.GetChild(0).GetComponent<RectTransform>();
        energyBar = transform.GetChild(0).GetComponent<Image>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    void Update() {
        energyBarRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, player.Energy/100f * length);
        energyBar.color = Color.Lerp(new Color(1f, 1f, 0.4f, 0.5f), new Color(1f, 1f, 0.4f, 0.5f), player.Energy/100f);

        containerRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, player.maxEnergy/100f * length);
    }
}
