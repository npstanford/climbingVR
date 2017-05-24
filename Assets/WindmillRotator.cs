using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindmillRotator : MonoBehaviour {

    public float RotationRate;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        this.transform.RotateAround(transform.position, transform.up, RotationRate * Time.deltaTime);
	}
}
