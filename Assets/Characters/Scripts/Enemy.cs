using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : Mob
{
    /*Stats*/
    public float enemyVision = 10f;
    public float damage = 20f;
    public float attackRange = 5f;
    public float chaseRange = 2f;
    public float chaseSpeed = 7f;
    public float wanderSpeed = 2f;

    public Vector3 attackHBCenter;
    public Vector3 attackHBSize = Vector3.one;

    /* Loot */
    public float probDrobLoot = 0.5f;
    public GameObject lootOrbPrefab;
    public List<Inventory> lootList;

    /* Navigation */
    protected NavMeshAgent agent;
    protected Transform player;
    protected Vector3 originalPosition;

    /*Audio*/
    public List<GameObject> randomSounds;
    public GameObject swingSound;

    /* Effects */
    public GameObject splatter;

    /* States */
    protected bool attacking;
    protected bool persuing;
    protected bool testPlayerHit;
    protected bool visionChecked = false;
    
    /* Misc */
    protected Animator animator;
    System.Random random;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        originalPosition = transform.position;

        animator = GetComponentInChildren<Animator>();

        vulnerable = true;
        alive = true;
        health = maxHealth;
        StartCoroutine(MakeRandomSound());
        agent.destination = transform.position;

        random = new System.Random(GetInstanceID() * System.Environment.TickCount);
    }

    void Update()
    {
        if(alive) {
            animator.SetFloat("Speed", agent.velocity.magnitude/7f);

            if(persuing) {
                Persue();

            } else {
                Wander();

            }

            if(testPlayerHit) {
                TestPlayerHit();
            }
        }
    }

    void StartPersuing() {
        
        persuing = true;
    }

    void Persue() {

        agent.speed = chaseSpeed;
        agent.SetDestination(player.position);

        float dist = Vector3.Distance(transform.position, player.position);
        
        if(dist < attackRange) {
            if(!attacking)
                StartCoroutine(Attack());
                
            Vector3 direction = player.position - transform.position;
            float angle = (Vector3.Angle(direction, Vector3.right)>90?-1f:1f) * Vector3.Angle(direction, Vector3.forward);
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.up);
        }

        if(dist < chaseRange) {
            agent.isStopped = true;
        } else {
            agent.isStopped = false;
        }

    }

    void Wander() {
        agent.speed = wanderSpeed;

        if(agent.remainingDistance <= 0.01f || Vector3.Distance(transform.position, originalPosition) > 10f) {
            agent.SetDestination(originalPosition + new Vector3((float)random.NextDouble() * 5f - 2.5f, 0f, (float)random.NextDouble() * 5f - 2.5f));
        }

        if(!visionChecked) {
            CheckVision();
            StartCoroutine(WaitForVisionCheck());
        }
    }

    public virtual IEnumerator Attack() {
        animator.SetTrigger("Attack");
        attacking = true;

        yield return new WaitForSeconds(1.3f);
        Instantiate(swingSound, transform.position, Quaternion.identity);

        testPlayerHit = true;

        yield return new WaitForSeconds(0.2f);

        testPlayerHit = false;

        yield return new WaitForSeconds(1f);

        attacking = false;
    }

    void TestPlayerHit() {
        Collider[] colls = Physics.OverlapBox(transform.position + transform.rotation * attackHBCenter, attackHBSize/2f, transform.rotation);

        foreach(Collider coll in colls) {
            if(coll.transform.tag == "Player") {
                coll.GetComponent<Player>().TakeDamage(damage);
                StartCoroutine(coll.GetComponent<Player>().StartInvincability());
                testPlayerHit = false;
            }
        }
    }

    private void DropLoot() {
        System.Random random = new System.Random();
        if(lootList.Count > 0) {
            bool willDropLoot = random.NextDouble() <= probDrobLoot;

            if(willDropLoot) {
                int randInv = (int)Mathf.Floor((float)random.NextDouble() * lootList.Count);

                InventoryController lootInv = Instantiate(lootOrbPrefab, transform.position, Quaternion.identity).GetComponent<InventoryController>();
                lootInv.InventoryValues = lootList[randInv];
            }
        }
    }

    public IEnumerator MakeRandomSound() {
        
        if(randomSounds.Count > 0) {
            System.Random random = new System.Random();
            
            int rand = (int)Mathf.Floor((float)random.NextDouble() * randomSounds.Count);
            Instantiate(randomSounds[rand], transform.position, Quaternion.identity);

            yield return new WaitForSeconds((float)random.NextDouble() * 5f + 0.5f);
            StartCoroutine(MakeRandomSound());
        }
    }

    void CheckVision() {
        Vector3 eyesPos = transform.position + Vector3.up * 2f;
        Vector3 direction = player.position - eyesPos;

        if(direction.magnitude > enemyVision)
            return;

        RaycastHit[] hits = Physics.RaycastAll(eyesPos, direction, direction.magnitude);

        bool detection = false;
        foreach(RaycastHit hit in hits) {
            if(hit.collider.tag == "Enemy" || hit.collider.tag == "EnemyBody" || hit.collider.tag == "Player")
                continue;
            
            detection = true;
            break;
        }

        if(!detection) {
            StartPersuing();
        }
        visionChecked = true;
    }

    IEnumerator WaitForVisionCheck() {
        yield return new WaitForSeconds(0.1f);
        visionChecked = false;
    }

    public void TakeBulletDamage(float damage, Vector3 hitPoint, Vector3 direction) {

        Instantiate(splatter, hitPoint, Quaternion.FromToRotation(Vector3.forward, direction));

        this.TakeDamage(damage);
        persuing = true;
    }
	
	public override IEnumerator Die()
	{
		animator.SetTrigger("Die");
        agent.isStopped = true;
        
        DropLoot();

        yield return new WaitForSeconds(4f);

		Destroy(gameObject);
	}

    void OnDrawGizmosSelected ()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, enemyVision);

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position + attackHBCenter, attackHBSize);
    }
}
