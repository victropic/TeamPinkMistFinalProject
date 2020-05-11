using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Mob
{
    /* Input */
    bool fire;
    bool reload;
    float switchItem;
    bool aim;

    /* Game Stats */
    public float maxEnergy;
    public float runningDrainRate = 20f;
    float energy;
    
    /* Status Effects */
    public List<StatusEffect> statusEffects;
    private List<float> durationLeft;

    /* States */
    private int currentItem;
    private float switchingTimer;

    private bool justStartedAiming;
    private bool justStoppedAiming;

    private float iaCheckTimer;

    /* Misc */
    Transform hand;
    FPSController fps;
    Transform upperBody;
    GameController gameController;

    public InventoryController handInventoryContr;
    public InventoryController bagInventoryContr;

    void Start()
    {
        upperBody = transform.GetChild(1);
        hand = upperBody.GetChild(0);
        
        handInventoryContr = transform.GetChild(2).GetComponent<InventoryController>();
        bagInventoryContr = transform.GetChild(3).GetComponent<InventoryController>();
        handInventoryContr.inventoryChangeDelegate += OnInventoryChange;
        
        fps = GetComponent<FPSController>();
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();

        statusEffects = new List<StatusEffect>();
        durationLeft = new List<float>();

        currentItem = 0;
        health = maxHealth;
        energy = maxEnergy;
        vulnerable = true;
        alive = true;

        EquipCurrentItem();
    }

    void Update()
    {
        /*Update player input*/
        fire = Input.GetButtonDown("Fire1");
        reload = Input.GetButtonDown("Reload");
        switchItem = Input.GetAxis("Mouse ScrollWheel");
        aim = Input.GetButton("Aim");
        
        if(!gameController.UIMode && alive) {
            if(transform.position.y < -100f)
                TakeDamage(health);

            /* Fire */
            if(fire) {
                UseCurrentTool();
            }

            /* Reload */
            if(reload) {
                ReloadCurrentTool();
            }

            /*Switch item (scroll wheel)*/
            if(Mathf.Abs(switchItem) > 0f && switchingTimer <= 0f) {
                switchingTimer = 0.1f;
                int direction = (switchItem > 0f)?1:-1;
                currentItem = mod((currentItem + direction), handInventoryContr.Length);

                EquipCurrentItem();

            }
            
            if(switchingTimer > 0f)
                switchingTimer -= Time.deltaTime;
            
            /* Aiming */
            try {
                GameObject itemObj = hand.GetChild(0).gameObject;

                if(itemObj) {
                    Weapon weapon = itemObj.GetComponent<Weapon>();

                    if(aim && justStartedAiming) {
                        weapon.StartAiming();
                    } else if(!aim && justStoppedAiming) {
                        weapon.StopAiming();
                    }
                }
            } catch {

            }

            if(aim) {
                if(justStartedAiming) {
                    justStartedAiming = false;
                    
                }
                justStoppedAiming = true;
            } else {
                if(justStoppedAiming) {
                    justStoppedAiming = false;
                }
                justStartedAiming = true;
            }
            
        }

        UpdateStatusEffects();

        if(energy > 0f) {
            if(fps.Running) {
                TakeEnergy(Time.deltaTime * runningDrainRate);
            }
        } else {
            if(fps.canRun) {
                StartCoroutine(fps.Rest());
            }
        }

        if(!fps.Running) {
            TakeEnergy(Time.deltaTime * - 5f);
        }

    }

    void FixedUpdate() {
        iaCheckTimer -= Time.deltaTime;

        if (!gameController.UIMode) {
            if(iaCheckTimer < 0f) { 
                iaCheckTimer = 0.1f;

                /*Check if player crosshair is hovering over interactive object*/
                RaycastHit[] rchs = Physics.RaycastAll(upperBody.position, upperBody.forward, 3f);
                bool anyInteractive = false;
                foreach(RaycastHit rch in rchs) {
                    string tag = rch.collider.tag;
                    if(tag == "Interactive") {
                        gameController.interactUI.Show();
                        anyInteractive = true;
                        gameController.CurrentInteractive = rch.collider.gameObject;
                        break;
                    }
                }
                if(!anyInteractive) {
                    gameController.interactUI.Hide();
                    gameController.CurrentInteractive = null;
                }
            }
        }
    }

    public void OnInventoryChange() {
        EquipCurrentItem();
    }

    private void UseCurrentTool() {
        if(hand.childCount <= 0 || hand.GetChild(0).GetComponent<Tool>() == null)
            return;
        Tool tool = hand.GetChild(0).GetComponent<Tool>();

        tool.Use();
    }

    private void ReloadCurrentTool() {
        if(hand.childCount <= 0 || hand.GetChild(0).GetComponent<Tool>() == null)
            return;

        Tool tool = hand.GetChild(0).GetComponent<Tool>();

        /* Find Ammo in inventory if Refillable*/
        if(tool.Item.itemInfo.GetType() == typeof(Refillable)) {

            int amount = ((Refillable)tool.Item.itemInfo).maxAmmo - ((RefillableItem)tool.Item).ammo;
            if(amount > 0) {
                int amountFound = handInventoryContr.SearchAndRemove(((Refillable)tool.Item.itemInfo).ammoItemInfo, amount);
                if(amount - amountFound > 0) {
                    amountFound += bagInventoryContr.SearchAndRemove(((Refillable)tool.Item.itemInfo).ammoItemInfo, amount - amountFound);
                }

                if(amountFound > 0) {
                    tool.Refill(amountFound, handInventoryContr);
                }
            }
        }
    }

    public void EquipCurrentItem() {
        
        Item item = handInventoryContr.GetItem(currentItem);

        if(hand.childCount > 0) {
            Tool tool = hand.GetChild(0).GetComponent<Tool>();

            if(tool.Item.Equals(item)) {
                return;
            } else
                Destroy(hand.GetChild(0).gameObject);
        }

        if(item != null && (item.itemInfo.GetType() == typeof(Equipable) || item.itemInfo.GetType().IsSubclassOf(typeof(Equipable)))) {
            Equipable equipable = (Equipable)item.itemInfo;
            GameObject itemObj = Instantiate(equipable.toolObject, Vector3.zero, Quaternion.identity);
            itemObj.GetComponent<Tool>().Item = item;

            itemObj.transform.SetParent(hand);
            itemObj.transform.localPosition = Vector3.zero;
            itemObj.transform.localRotation = Quaternion.identity;
        }
    }

    public void DestroyCurrentItem() {
        handInventoryContr.Remove(currentItem);
    }

    public float GetWeaponCharge() {
        if(hand.transform.childCount > 0) {
            GameObject obj = hand.transform.GetChild(0).gameObject;
            Weapon weapon = obj.GetComponent<Weapon>();
            if(weapon != null) {
                return weapon.GetCharge();
            }
        }

        return 0f;
    }

    public float Energy {
        get { return energy; }
    }

    public void TakeEnergy(float cost) {
        energy = Mathf.Clamp(energy - cost, 0f, 100f);
    }

    private void UpdateStatusEffects() {
        for(int i = 0; i < statusEffects.Count; i++) {
            StatusEffect s = statusEffects[i];
            switch(s.statEffected) {
                case StatusEffect.StatEffected.Health:
                    TakeChipDamage(-(s.amount/s.duration) * Time.deltaTime);
                    break;
                case StatusEffect.StatEffected.Energy:
                    TakeEnergy(-(s.amount/s.duration) * Time.deltaTime);
                    break;
            }

            durationLeft[i] -= Time.deltaTime;
            if(durationLeft[i] < 0) {
                statusEffects.RemoveAt(i);
                durationLeft.RemoveAt(i);
            }
        }
    }

    public void AddStatusEffect(StatusEffect statusEffect) {
        if(!statusEffect.stackable) {
            foreach(StatusEffect s in statusEffects) {
                if(statusEffect.statusName == s.statusName) {
                    return;
                }
            }
        }
        statusEffects.Add(statusEffect);
        durationLeft.Add(statusEffect.duration);
    }

    public override IEnumerator Die() {
        fps.Frozen = true;
        yield return new WaitForSeconds(0.01f);
        gameController.ShowDeathSequence();
    }
    
    int mod(int x, int m) {
        return (x%m + m)%m;
    }
}

