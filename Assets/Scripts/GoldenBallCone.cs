using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldenBallCone : MonoBehaviour {


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        GoldenBall gb = other.GetComponent<GoldenBall>();
        if (gb != null)
        {
            gb.Attach(transform.position);
            SphereCollider ourCollider = GetComponentInChildren<SphereCollider>();
            if (ourCollider != null)
            {
                ourCollider.enabled = false;
            }
        }
    }

    /*
    private void OnTriggerExit(Collider other)
    {
        GoldenBall gb = other.GetComponent<GoldenBall>();
        if (gb != null)
        {
            gb.Detach();
        }
    }
    */
}
