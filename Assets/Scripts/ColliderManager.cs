﻿using UnityEngine;
using System.Collections;

public class ColliderManager : MonoBehaviour {
    /*
     * Originally, this class ONLY implemented logic to move the box collider around inside of the camera rig
     * I am extending it now to also detect when that collider is touching ground and to update the player's status as such
     * 
     */ 


    public BoxCollider playerCollider;
    public GameObject OverheadCollider;
    public GameObject RunningColliderTop;
    public GameObject RunningColliderBottom;
    public GameObject playerHead;
    public float OverheadColliderHeight;
    public float RunningColliderTopHeight;
    public float RunningColliderBottomHeight;

    public GameObject displayCube;
    public GameController gc;

    public Teleporter teleporter; 

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void FixedUpdate ()
    {
        //assumption here is that in the local coordinates of the rig that the ground is a y=0
        Vector3 colliderCenter = playerHead.transform.localPosition;
        colliderCenter.y = colliderCenter.y / 2;
        playerCollider.center = colliderCenter;

        playerCollider.size = new Vector3(0.2f, colliderCenter.y * 2, 0.2f);

        OverheadCollider.transform.position = playerHead.transform.position + Vector3.up * OverheadColliderHeight;
        RunningColliderTop.transform.position = playerHead.transform.position + Vector3.up * RunningColliderTopHeight;
        RunningColliderBottom.transform.position = playerHead.transform.position + Vector3.up * RunningColliderBottomHeight;

        //update display cube for testing purposes
        displayCube.transform.localPosition = colliderCenter;
        displayCube.transform.localScale = playerCollider.size;
    }

    void OnCollisionStay (Collision other)
    {
        //if (other.CompareTag("Ground"))
        {
            gc.PlayerIsTouchingGround = true;
            TeleportLocation tl = other.collider.GetComponent<TeleportLocation>();
            if (tl != null)
            {
                teleporter.LastTeleportLocation = tl;
            }
        }
    }

    void OnCollisionExit (Collision other)
    {
        //if (other.CompareTag("Ground"))
        {
            gc.PlayerIsTouchingGround = false;
        }
    }

}
