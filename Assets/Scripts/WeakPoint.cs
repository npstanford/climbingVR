using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeakPoint : MonoBehaviour {
    public Enemy enemy;

    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.CompareTag("Grapple"))
        {

            //enemy.Stun();
        }
    }

}
