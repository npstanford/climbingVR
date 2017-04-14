using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingPlatform : MonoBehaviour {

    public bool RotationEnabled = true;
    public float RotationRate;


	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
        if (RotationEnabled)
        {
            this.transform.Rotate(0.0f, RotationRate * Time.deltaTime, 0.0f, Space.Self);
        }
	}
}
