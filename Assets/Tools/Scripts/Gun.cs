using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : Weapon
{
    public float cooldown;
	public int shootRange = 100; //shooting range of gun
    public float aimSpeed;
    public float reloadTime = 1f;

    GameObject meshObject;
    GameObject hand;
    Animator animator;
    public Vector3 aimedPosition = new Vector3(0f, -0.1f, 0.3f);
    Vector3 unaimedPosition;

    public GameObject shotAudio;
    public GameObject dryAudio;
    public GameObject reloadAudio;

    private bool canShoot;
    private float cooldownTimer;
    private float centeredness;
    
    // Start is called before the first frame update
    void Start()
    {
        meshObject = transform.GetChild(0).gameObject;
        animator = meshObject.GetComponent<Animator>();
        hand = transform.parent.gameObject;
        unaimedPosition = hand.transform.localPosition;
        canShoot = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(cooldownTimer > 0f) {
            cooldownTimer -= Time.deltaTime;
        }

        if(aiming) {
            centeredness = Mathf.Clamp(centeredness + Time.deltaTime * aimSpeed, 0f, 1f);
        } else {
            centeredness = Mathf.Clamp(centeredness - Time.deltaTime * aimSpeed, 0f, 1f);
        }

        hand.transform.localPosition = Vector3.Lerp(unaimedPosition, aimedPosition, centeredness);
    }

    public override void Use() {
        if(cooldownTimer <= 0f && canShoot && ((RefillableItem)item).ammo > 0) {
            animator.SetTrigger("Shoot");
            Instantiate(shotAudio, transform.position, Quaternion.identity);
            cooldownTimer = cooldown;
            ((RefillableItem)item).ammo -= 1;

            RaycastHit[] raycastHits =  Physics.RaycastAll(transform.position, transform.forward);
            foreach(RaycastHit rch in raycastHits) {
                if(rch.collider.gameObject.tag == "EnemyBody") {
					
                    float multiplier = 1f;
                    if(rch.collider.name == "HeadHC") {
                        multiplier = 2f;
                        Debug.Log("Headshot");
                    }

					//Decrease health
					Enemy enemy = rch.collider.gameObject.transform.root.GetComponent<Enemy>();
                    enemy.TakeBulletDamage(damage * multiplier, rch.point, transform.forward);

                    break;
                }
            }
            

        } else if(((RefillableItem)item).ammo <= 0) {
            animator.SetTrigger("Dry");
            Instantiate(dryAudio, transform.position, Quaternion.identity);
        }
    }

    public override void Refill(int amount, InventoryController ic) {
        Instantiate(reloadAudio, transform.position, Quaternion.identity);
        animator.SetTrigger("Reload");
        animator.ResetTrigger("Dry");
        StartCoroutine(EndRefill(amount, ic));
        canShoot = false;
    }

    private IEnumerator EndRefill(int amount, InventoryController ic) {
        yield return new WaitForSeconds(reloadTime);

        if(((RefillableItem)item).ammo + amount > ((Refillable)item.itemInfo).maxAmmo)
            ((RefillableItem)item).ammo = ((Refillable)item.itemInfo).maxAmmo;
        else
            ((RefillableItem)item).ammo += amount;

        if(ic.inventoryChangeDelegate != null)
            ic.inventoryChangeDelegate();

        canShoot = true;
    }

    public override void StartAiming() {
        aiming = true;
        centeredness = 0f;
    }

    public override void StopAiming() {
        aiming = false;
    }

    public override float GetCharge() {
        return centeredness;
    }
}
