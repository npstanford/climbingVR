using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Run : MonoBehaviour {
    /*
    public enum DirectionMethod { Gaze, Controller};


    //public float ArmSpeed;

    //public float StepSize = 1.0f;
   // private bool Stepped = false;
   // public float StepTime = 0.1f;
    //public bool UseTrigStep = true;
    //public bool IsRunning = true;
    //public GameObject AvgMarker;
    //public bool CoroutineIsRunning = false;
    public float HeadVerticalVelocity; //this value is always set to the average of the values in velocity array
    public GameObject PlayerHead;
    public int HeadVelocityArraySize;
    public int MoveDirectionArraySize;
    public float WalkThreshold;
    public float WalkVelocity = 2.0f;
    public float JogVelocity = 5.0f;
    public float sprintVelocity = 10.0f;
    public DirectionMethod DM = DirectionMethod.Gaze;
 
    
    


    public GameObject Player;
    public Rigidbody Room;

    private float _oldHeadPosition;
    private float _newHeadPosition;
    private float[] _headVerticalVelocities; //this stores all of the instaneous velocitys
    private Vector3[] _moveDirections;
    private int _velocityIndex = 0;
    private int _directionIndex = 0;
    private int i = 0; //just for debugging

    void Start()
    {
        //Stepped = false;

        _oldHeadPosition = PlayerHead.transform.position.y;
        _newHeadPosition = PlayerHead.transform.position.y;
        _headVerticalVelocities = new float[HeadVelocityArraySize];

        _moveDirections = new Vector3[MoveDirectionArraySize];
    }

    void Update()
    {
        //calculate head vertical velocity so that it is always available for us

        _oldHeadPosition = _newHeadPosition;
        _newHeadPosition = PlayerHead.transform.position.y;
        if (gc.PlayerIsTouchingGround)
        {
            _headVerticalVelocities[_velocityIndex % HeadVelocityArraySize] = (_newHeadPosition - _oldHeadPosition) / Time.deltaTime;
        } else
        {
            _headVerticalVelocities[_velocityIndex % HeadVelocityArraySize] = 0.0f;
        }
        _velocityIndex += 1;

        HeadVerticalVelocity = 0;

        foreach (float v in _headVerticalVelocities)
        {
            HeadVerticalVelocity += v;
        }

        HeadVerticalVelocity = HeadVerticalVelocity / HeadVelocityArraySize;

  
        if (i % 100 == 0)
        {
            //Debug.Log("HeadVelocity: " + HeadVerticalVelocity);


        }

        i += 1;

    }

    public void Stop()
    {
        for( i=0; i<MoveDirectionArraySize; i++)
        {
            _moveDirections[i] = Vector3.zero;
        }

        _directionIndex = 0;
    }

    public void Step(ControllerState lController)
    {/*
        if (gc.PlayerIsTouchingGround)
        {

            _moveDirections[_directionIndex % MoveDirectionArraySize] = new Vector3(lController.controller.transform.forward.x, 0.0f, lController.controller.transform.forward.z).normalized;
            _directionIndex += 1;

            int count = 0;
            Vector3 moveDirection = Vector3.zero;


            foreach(Vector3 v in _moveDirections)
            {
                if (v != Vector3.zero)
                {
                    count += 1;
                    moveDirection.x = v.x;
                    moveDirection.z = v.z;
                }
            }

            moveDirection.x = moveDirection.x / count;
            moveDirection.z = moveDirection.z / count;
            moveDirection = moveDirection.normalized;

            //Vector3 moveDirection = new Vector3(lController.controller.transform.forward.x, 0.0f, lController.controller.transform.forward.z).normalized;



            if (Mathf.Abs(HeadVerticalVelocity) > WalkThreshold)
            {
                Room.transform.position += moveDirection * GetMoveVelocity(HeadVerticalVelocity) * Time.deltaTime;
            }
            
            
            
            
            
            //note commented out the below to try to get the running mechanic to work

            /*
            Vector3 walkDirection;

            Vector3 AvgArmPosition = Vector3.zero;

            AvgArmPosition.x = (lController.RunTopPoint.x + lController.RunBottomPoint.x + rController.RunTopPoint.x + rController.RunBottomPoint.x) / 4;
            AvgArmPosition.y = (lController.RunTopPoint.y + lController.RunBottomPoint.y + rController.RunTopPoint.y + rController.RunBottomPoint.y) / 4;
            AvgArmPosition.z = (lController.RunTopPoint.z + lController.RunBottomPoint.z + rController.RunTopPoint.z + rController.RunBottomPoint.z) / 4;

            walkDirection = (AvgArmPosition - Player.transform.localPosition);
            walkDirection = Room.transform.TransformDirection(walkDirection);
            walkDirection = new Vector3(walkDirection.x, 0, walkDirection.z);
            walkDirection = walkDirection.normalized;


            //delete later juts for debugging
            AvgMarker.transform.localPosition = AvgArmPosition;

            float walkDistance;
            //trim walk so it doesn't go through a wall
            //TODO with new walking mechanism that is time and trig based, this no longer works
            RaycastHit hit; //may need to add layer mask that ignores controllers and some other things
            if (Physics.Raycast(Player.transform.position, walkDirection, out hit, StepSize))
            {
                float d = Vector3.Distance(hit.point, Player.transform.position);
                if (d < StepSize)
                {
                    walkDistance = d;
                }
                else
                {
                    walkDistance = StepSize;
                }

            }
            else
            {
                walkDistance = StepSize;

            }

            if (!IsRunning)
            {
                IsRunning = true;
                StartCoroutine(WalkAndBlur(Room.transform.position + walkDirection * walkDistance));
            }

        }
 
        */


    //}

    /*
    IEnumerator WalkAndBlur(Vector3 finalPosition)
    {

        //FallingFilter.enabled = true;

        float elapsedTime = 0;


        Vector3 startPosition = Room.transform.position;

        Vector3 walkingDirection = (finalPosition - startPosition).normalized;


        float x = 0;

        while (elapsedTime < StepTime)
        {
            elapsedTime += Time.deltaTime;

            if (UseTrigStep)
            {
                //use trig functions Sin(0) -> Sin (pi)
                // some bearing vector normalized times a small step size 
                x = (elapsedTime / StepTime) * Mathf.PI / 2 - Mathf.PI / 4.0f;

                //TODO: this has the problem that you'll walk super slowly towards a wall... but I can fix that later. Solution is to make StepTime a function of walking distance
                Vector3 newPosition = Vector3.Lerp(startPosition, finalPosition, (Mathf.Tan(x) / 2 + 0.5f));
                Room.transform.position = newPosition;

                //Room.transform.position += walkingDirection * Mathf.Sin((elapsedTime / StepTime) * Mathf.PI);


                Room.velocity = Vector3.zero;
                yield return null;
            }
            else
            {
                x = (elapsedTime / StepTime);
                Room.transform.position = Vector3.Lerp(startPosition, finalPosition, x);
            }


        }
        IsRunning = false;

    }
    */

        /*
    public float GetMoveVelocity(float HeadVerticalVelocity)
    {
        if (HeadVerticalVelocity <0.01f)
        {
            return 0.0f;
        }
        else if (HeadVerticalVelocity < .05f)
        {
            return WalkVelocity;
        } else
        {
            return JogVelocity;
        }
    }
    */
}
