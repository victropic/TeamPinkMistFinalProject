using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{

    public float length;
    RectTransform healthBarRT;
    RectTransform containerRT;
    Image healthBar;
    Image container;
    Player player;

    void Start()
    {
        containerRT = GetComponent<RectTransform>();
        container = GetComponent<Image>(); 
        healthBarRT = transform.GetChild(0).GetComponent<RectTransform>();
        healthBar = transform.GetChild(0).GetComponent<Image>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    void Update() {
        healthBarRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, player.Health/100f * length);
        healthBar.color = Color.Lerp(new Color(0.75f, 0f, 0f, 0.5f), new Color(1f, 1f, 1f, 0.5f), player.Health/100f);

        containerRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, player.maxHealth/100f * length);
    }
}
