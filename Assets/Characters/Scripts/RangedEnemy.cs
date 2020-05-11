using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedEnemy : Enemy
{
    public GameObject projectilePref;

    public override IEnumerator Attack() {
        attacking = true;

        animator.SetTrigger("Attack");

        yield return new WaitForSeconds(1f);

        Vector3 throwPos = transform.position + Vector3.up * 1.5f + transform.forward;

        GameObject projectile = Instantiate(projectilePref, throwPos, Quaternion.identity) as GameObject;
        projectile.GetComponent<Rigidbody>().AddForce((player.position - throwPos).normalized * 50f, ForceMode.Impulse);

        yield return new WaitForSeconds(2.5f);

        attacking = false;
    }
}
