using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
    public List<Enemy> enemies;
    public float radius;
    public float spawnAmount = 1f;
    public float duration;
    
    float spawnTimer;
    float timer;
    System.Random random;

    void Start() {
        random = new System.Random();
        spawnTimer = 0f;
        timer = duration;
    }

    void Update() {
            spawnTimer -= Time.deltaTime;
            timer -= Time.deltaTime;
            if(spawnTimer <= 0f && timer > 0f && enemies.Count > 0) {
                Vector3 displacement = Quaternion.AngleAxis((float)random.NextDouble() * 360f, Vector3.up) * 
                        new Vector3(0f, 0f, (float)random.NextDouble() * radius);

                int randInd = (int)Mathf.Floor((float)random.NextDouble() * enemies.Count);
                Instantiate(enemies[randInd], transform.position + displacement,
                    Quaternion.identity * Quaternion.AngleAxis((float)random.NextDouble(), Vector3.up));
                spawnTimer = duration/spawnAmount;
            }

    }

    void OnDrawGizmosSelected ()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, radius);
    }

}
