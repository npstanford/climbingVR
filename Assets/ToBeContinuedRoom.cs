using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToBeContinuedRoom : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("entered dark room");
        if (other.CompareTag("Player"))
        {
            Debug.Log("ambient intensity turned to zero");
            RenderSettings.ambientIntensity = 0.0f;
        }
    }

}
