using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpCollisionSounds : MonoBehaviour {

    public AudioSource CollisionThudSound;
    private Vector3 prevPos;
    private Vector3 curPos;
    private float speed;

	// Use this for initialization
	void Start () {

	}

    private void Update()
    {
        prevPos = curPos;
        curPos = transform.position;
        speed = (curPos - prevPos).magnitude / Time.deltaTime;
    }

    private void OnCollisionEnter(Collision other)
    {
        CollisionThudSound.volume = Mathf.Min(speed / 10f, 1f);
        CollisionThudSound.Play();
    }

    private void OnTriggerEnter(Collider other)
    {
        CollisionThudSound.volume = Mathf.Min(speed / 10f, 1f);
        CollisionThudSound.Play();
    }

}
