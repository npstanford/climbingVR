﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathNet.Numerics;

public class BodyMovementCollector : MonoBehaviour {
    public ControllerState LeftController;
    public ControllerState RightController;
    public LineRenderer RightLR;
    public LineRenderer LeftLR;
    public GameObject TopOfHead;
    public int size;
    public bool ShowDirectioNRays = false;
    public ColliderManager cm;

    public BodyTrackingTimeSeries RightArm;
    public BodyTrackingTimeSeries LeftArm;
    public BodyTrackingTimeSeries Head;

    public float HeadSpeed;
    public Vector3 HeadDirection;
    public float RightArmSpeed;
    public Vector3 RightArmDirection;
    

    /*
     * Arms... imagine they are moving in a vertical plane. We record the xz vectors and average over them. We then combine left
     * and right arms with equal weight to get the overall movement direction.
     * 
     * I am going to need primitives of a time series that can give statistics of that time series. At least mean, possible variance.
     * 
     * Head movement, we collect head position to calculate velocity
     * 
     * 
     * So I think we have six components to work with. For head, left and right arms, we have "speed" and "direction".
     * - For first version, we just average the directions together (weighted average -- later heuristics can adjust weights)
     * - For speed, we also do a weighted average and then have a function (probably non-linear) that translates that into forward movement. 
     * - There should be a signal that if the x and z of the head are changing (sufficiently) ,then we shouldn't run. 
     * 
     * In the future, we can pull whether the glider is currently deployed or whether the hookshot is shown. 
     * 
     */


	// Use this for initialization
	void Start () {
        RightArm = new BodyTrackingTimeSeries(size);
        LeftArm = new BodyTrackingTimeSeries(size);
        Head = new BodyTrackingTimeSeries(size);
        if (ShowDirectioNRays)
        {
            RightLR.enabled = true;
            LeftLR.enabled = true;
        }
    }
	
	// Update is called once per frame
	void Update () {
        //for the controllers, I want the vector that is in the same plane as the plane perpendicular to the ground and the forward vector
        // but has no y component

        //if forward.y is > .5 (meaning the player is holding the controllers more vertically) then use -up
        //but if it is <=.5 (meaning the controllers are more horizontal) then use 
        //to record for posterity -- the current best experience i had was when the direction vector was based on -up with no y component
        //note, another possibility might be to average these directions together 


        Vector3 rDirection;
        Vector3 lDirection;

        if (RightController.transform.forward.y > .5)
        {
            rDirection = new Vector3(-RightController.transform.up.x, 0.0f, -RightController.transform.up.z);
        } else
        {
            rDirection = new Vector3(RightController.transform.forward.x, 0.0f, RightController.transform.forward.z);
        }

        if(LeftController.transform.forward.y > .5)
        {
            lDirection = new Vector3(-LeftController.transform.up.x, 0.0f, -LeftController.transform.up.z);
        } else
        {
            lDirection = new Vector3(LeftController.transform.forward.x, 0.0f, LeftController.transform.forward.z);
        }

        if (ShowDirectioNRays)
        {
            RightLR.SetPosition(0, RightController.transform.position);
            RightLR.SetPosition(1, RightController.transform.position + rDirection.normalized);
            LeftLR.SetPosition(0, LeftController.transform.position);
            LeftLR.SetPosition(1, LeftLR.transform.position + lDirection.normalized);
        }

        RightArm.Update(RightController.transform.position, rDirection.normalized, cm.PlayerIsTouchingGround);
        LeftArm.Update(LeftController.transform.position, lDirection.normalized, cm.PlayerIsTouchingGround);
        Head.Update(TopOfHead.transform.position, TopOfHead.transform.forward, cm.PlayerIsTouchingGround);


        RightArmSpeed = RightArm.speed;
        RightArmDirection = RightArm.direction;
        HeadSpeed = Head.speed;
        HeadDirection = Head.direction;

	}

    public class BodyTrackingTimeSeries
    {
        private Vector3[] directions;
        private Vector3[] positions;
        private float[] speeds;
        public Vector3 direction;
        public float speed;

        private int i;
        private int size;

        public BodyTrackingTimeSeries(int size)
        {
            directions = new Vector3[size];
            positions = new Vector3[size];
            speeds = new float[size];
            direction = Vector3.zero;
            speed = 0.0f;
            i = 0;
            this.size = size;

            for (int j = 0; j < size; j++)
            {
                directions[j] = Vector3.zero;
                positions[j] = Vector3.zero;
                speeds[j] = 0.0f;
            }

        }

        /*
         * Takes the forward direction of controller, head, etc. and the most recent position 
         */
        public void Update(Vector3 position, Vector3 forward, bool PlayerIsTouchingGround)
        {
            //If player is not touching the ground, don't fill in data
            if (!PlayerIsTouchingGround)
            {

                    positions[i] = Vector3.zero;
                    speeds[i] = 0f;
                    directions[i] = Vector3.zero;
                    i = Mod(i + 1, size);
                return;
            }


            positions[i] = position;
            if (positions[Mod(i - 1, size)] != Vector3.zero)
            {
                speeds[i] = (positions[i].y - positions[Mod(i - 1, size)].y) / Time.deltaTime;
            } else
            {
                speeds[i] = 0.0f;
            }
            directions[i] = forward.normalized;

            Vector3 avgD = Vector3.zero;

            foreach (Vector3 d in directions)
            {
                avgD += d;
            }

            direction = (avgD / size).normalized;

            float avgS = 0f;

            foreach (float s in speeds)
            {
                avgS += Mathf.Abs(s);
            }

            speed = (avgS / size);


            //this is to catch the case that when you drop, your current position is calculated against the origin to find your speed
            if (positions[Mod(i - 1, size)] == Vector3.zero) {
                direction = Vector3.zero;
                speed = 0.0f;
            }
            i = Mod(i + 1, size);
        }

        int Mod(int a, int b)
        {
            return (a % b + b) % b;
        }
    }


}
