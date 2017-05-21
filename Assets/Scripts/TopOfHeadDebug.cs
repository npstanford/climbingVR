using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopOfHeadDebug : MonoBehaviour {

    public float x;
    public float y;
    public float z;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        x = transform.position.x;
        y = transform.position.y;
        z = transform.position.z;
	}
}
