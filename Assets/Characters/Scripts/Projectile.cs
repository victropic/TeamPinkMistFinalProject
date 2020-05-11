using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : LastingObject
{
    public float damage;

    void OnTriggerEnter(Collider coll) {
        if(coll.tag == "Player") {
            Player player = coll.GetComponent<Player>();
            player.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
