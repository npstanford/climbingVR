using UnityEngine;
using System.Collections;

public class TeleportLocation : MonoBehaviour {

    public GameObject TeleportObject;

	// Use this for initialization
	void Start () {
        TeleportObject.GetComponent<MeshRenderer>().enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
