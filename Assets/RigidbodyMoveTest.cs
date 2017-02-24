using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigidbodyMoveTest : MonoBehaviour {
    public Rigidbody Body;
    public GameObject StartPoint;
    public GameObject StopPoint;
	// Use this for initialization
	void Start () {
        Body = this.GetComponent<Rigidbody>();
        Body.transform.position = StartPoint.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
        Body.MovePosition(StopPoint.transform.position);
	}
}
