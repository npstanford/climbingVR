using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wind : MonoBehaviour {

    public Vector3 WindVelocity;
    public float WindSpeed;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        WindVelocity = transform.up * WindSpeed;

    }
}
