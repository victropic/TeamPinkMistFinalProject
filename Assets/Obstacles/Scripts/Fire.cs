using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : MonoBehaviour
{

    public float damage = 10f;
    public float onDuration = 1f;
    public float offDuration = 1f;
    public StatusEffect statusEffect;
    public float startDelay;

    float timer;
    bool isOn = false;

    ParticleSystem particle;
    AudioSource sound;

    void Start() {
        particle = GetComponent<ParticleSystem>();
        timer = startDelay;
        sound = GetComponent<AudioSource>();
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if(timer <= 0f) {
            if(isOn) {
                isOn = false;
                timer = offDuration;
                particle.Stop();
            } else {
                isOn = true;
                timer = onDuration;
                particle.Play();
                sound.Play();
            }
        }
    }

    void OnTriggerStay(Collider collider) {
        if(isOn && collider.tag == "Player") {
            Player player = collider.GetComponent<Player>();
            player.AddStatusEffect(statusEffect);
        }
    }
}
