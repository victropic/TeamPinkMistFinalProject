using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mob : MonoBehaviour
{
    public float maxHealth = 100f;
    public float invincibilityDuration = 0.1f;
    public float health;
    protected bool alive;
    protected bool vulnerable;

    public List<GameObject> hitSounds;
    
    public float Health {
        get { return health; }
    }

    void Start() {
        health = maxHealth;
        vulnerable = true;
    }

    public void TakeDamage(float damage) {
        if(alive && vulnerable) {
            health = Mathf.Clamp(health - damage, -1f, maxHealth);
            if(health <= 0) {
                alive = false;
                StartCoroutine(Die());
            }
            PlayRandomHitSound();
            StartCoroutine(StartInvincability());
        }
    }

    public void TakeChipDamage(float damage) {
        if(alive) {
            health = Mathf.Clamp(health - damage, -1f, maxHealth);
            if(health <= 0) {
                alive = false;
                StartCoroutine(Die());
            }
        }
    }

    private void PlayRandomHitSound() {
        if(hitSounds.Count > 0) {
            System.Random random = new System.Random();

            int rand = (int)Mathf.Floor((float)random.NextDouble() * hitSounds.Count);
            Instantiate(hitSounds[rand], transform.position, Quaternion.identity);
        }
    }

    public IEnumerator StartInvincability() {
        vulnerable = false;
        yield return new WaitForSeconds(invincibilityDuration);
        vulnerable = true;
    }


    public virtual IEnumerator Die() {
        
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }

}
