using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LastingObject : MonoBehaviour
{

    public float timeToLive;
    void Start()
    {
        StartCoroutine(Die(timeToLive));
    }

    IEnumerator Die(float time) {
        yield return new WaitForSeconds(time);

        Destroy(gameObject);
    }
}
