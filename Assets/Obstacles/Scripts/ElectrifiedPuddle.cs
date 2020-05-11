using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectrifiedPuddle : MonoBehaviour
{
    public StatusEffect statusEffect1;
    public StatusEffect statusEffect2;
    public PowerSource source;

    System.Random random;

    ParticleSystem[] particleSystems;
    AudioSource sound;

    bool checkPower = true;
    bool isPower = false;
    // Start is called before the first frame update
    void Start()
    {
        random = new System.Random();
        particleSystems = GetComponentsInChildren<ParticleSystem>();
        sound = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if(checkPower) {
            checkPower = false;
            StartCoroutine(CheckPower());
        }
    }

    IEnumerator CheckPower() {
        
        if(source != null && source.isOn) {
            TurnOn();
            isPower = true;
        } else {
            TurnOff();
            isPower = false;
        }

        yield return new WaitForSeconds(0.1f);
        checkPower = true;
    }

    void TurnOn() {
        foreach(ParticleSystem p in particleSystems) {
            if(!p.isPlaying)
                p.Play();
        }
        if(!sound.isPlaying)
            sound.Play();
    }

    void TurnOff() {
        foreach(ParticleSystem p in particleSystems) {
            p.Stop();
        }
        sound.Stop();
    }

    void OnTriggerStay(Collider coll) {
        if(isPower && coll.tag == "Player") {
            Player player = coll.GetComponent<Player>();

            StatusEffect se = (random.NextDouble()>0.5)?statusEffect1:statusEffect2;
            player.AddStatusEffect(se);
        }
    }
}
