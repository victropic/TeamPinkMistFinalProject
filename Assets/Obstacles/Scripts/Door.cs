using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : Interactable
{
    Collider coll;
    Animator animator;
    public bool isLocked;
    public ItemInfo key;

    bool checkSensor;

    Player player;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        coll = GetComponent<Collider>();
        checkSensor = true;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    void Update()
    {
        if(!isLocked && checkSensor) {
            Collider[] colls = Physics.OverlapBox(transform.position + Vector3.up * 1.5f, new Vector3(4f, 1.5f, 3f));
            bool detection = false;
            foreach(Collider c in colls) {
                if(c.tag == "Player" || c.tag == "Enemy") {
                    animator.SetBool("open", true);
                    coll.enabled = false;
                    detection = true;
                    break;
                }
            }

            if(!detection) {
                animator.SetBool("open", false);
                coll.enabled = true;
            }

            checkSensor = false;
            StartCoroutine(WaitToCheck());
        } else if(isLocked && checkSensor) {
            animator.SetBool("open", false);
            coll.enabled = true;

            checkSensor = false;
            StartCoroutine(WaitToCheck());
        }
    }

    public override void Interact() {
        if(isLocked && key != null) {
            int keyFound = player.handInventoryContr.SearchAndRemove(key, 1);
            if(keyFound > 0)
                isLocked = false;
            else {
                keyFound = player.bagInventoryContr.SearchAndRemove(key, 1);
                if(keyFound > 0)
                    isLocked = false;
            }
        }
    }

    IEnumerator WaitToCheck(){
        yield return new WaitForSeconds(0.1f);
        checkSensor = true;
    }
}
