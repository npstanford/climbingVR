using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathNet.Numerics;

public class BodyMovementCollector : MonoBehaviour
{
    public ControllerState LeftController;
    public ControllerState RightController;
    public LineRenderer RightLR;
    public LineRenderer LeftLR;
    public GameObject TopOfHead;
    public int size;
    public bool ShowDirectioNRays = false;
    public ColliderManager cm;
    public Transform Room;
    [HideInInspector]
    public bool NonRunningHeadMovement = false;

    [HideInInspector]
    public BodyTrackingTimeSeries RightArm;
    [HideInInspector]
    public BodyTrackingTimeSeries LeftArm;
    [HideInInspector]
    public BodyTrackingTimeSeries Head;

    /*
    [HideInInspector]
    public float HeadSpeed;
    [HideInInspector]
    public Vector3 HeadDirection;
    [HideInInspector]
    public float RightArmSpeed;
    [HideInInspector]
    public Vector3 RightArmDirection;
    */


    // Use this for initialization
    void Start()
    {
        //These three are used for running
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
    void Update()
    {
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
        }
        else
        {
            rDirection = new Vector3(RightController.transform.forward.x, 0.0f, RightController.transform.forward.z);
        }

        if (LeftController.transform.forward.y > .5)
        {
            lDirection = new Vector3(-LeftController.transform.up.x, 0.0f, -LeftController.transform.up.z);
        }
        else
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

        Vector3 TopOfHeadInRoomLocalSpace = Room.InverseTransformPoint(TopOfHead.transform.position);

        RightArm.Update(RightController.transform.localPosition, rDirection.normalized, cm.PlayerIsTouchingGround);
        LeftArm.Update(LeftController.transform.localPosition, lDirection.normalized, cm.PlayerIsTouchingGround);
        Head.Update(TopOfHeadInRoomLocalSpace, TopOfHead.transform.forward, cm.PlayerIsTouchingGround);


        NonRunningHeadMovement = Head.CheckForDucking();

        /*
        RightArmSpeed = RightArm.speed;
        RightArmDirection = RightArm.direction;
        HeadSpeed = Head.speed;
        HeadDirection = Head.direction;
        */


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


            positions[i] = position;
            if (positions[Mod(i - 1, size)] != Vector3.zero)
            {
                speeds[i] = (positions[i].y - positions[Mod(i - 1, size)].y) / Time.deltaTime;
            }
            else
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

            i = Mod(i + 1, size);
        }

        int Mod(int a, int b)
        {
            return (a % b + b) % b;
        }

        public bool CheckForDucking()
        {
            //looks to see if all of changes in position.y are the same across positions
            int j = i;

            bool GoingDown = (positions[j].y - positions[Mod(j + 1, size)].y > 0);
            for (int k = 0; k < size - 1; k++)
            {
                //basically, go a full loop through the buffer and check the sign of the change of position at each point
                // if it ever differs from the first one, return false
                if (GoingDown != (positions[Mod(j + k, size)].y - positions[Mod(j + k + 1, size)].y > 0))
                {

                    return false;
                }
            }
            return true; // i.e. all the changes in position are the same sign (meaning player is ducking or standing)
        }

        public void Clear()
        {
            i = 0;

            for (int j = 0; j < size; j++)
            {
                positions[j] = Vector3.zero;
                speeds[j] = 0f;
                directions[j] = Vector3.zero;
            }
        }

    }


}