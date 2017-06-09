using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.ImageEffects;

public class Walk : MonoBehaviour {

    public float StepSize = 1.0f;
    private bool Stepped = false;
    public float StepTime = 0.1f;
    public bool UseTrigStep = true;
    //public bool IsRunning = true;
    public VignetteAndChromaticAberration Vignette;
    public float MaxVignette;
    public BlurOptimized bo;
    public float GripDepletion;
    public AudioSource DashStepSound;

    public bool HasStepped = false; // used by intro sequence to see if player has successfully taken a dash step yet.


    public GameObject Player;
    public Rigidbody Room;

    private InputManager im;

    void Start () {
        Stepped = false;
        HasStepped = false;
        im = FindObjectOfType<InputManager>();
    }

    public void Step(SteamVR_TrackedObject controller) {
  //note commented out the below to try to get the running mechanic to work

            Vector3 walkDirection;

            /*
            if (IsRunning)
            {
                walkDirection = new Vector3(controller.transform.up.x, 0, controller.transform.up.z).normalized;
                walkDirection = -walkDirection;
            } else
            { */
                walkDirection = new Vector3(controller.transform.forward.x, 0, controller.transform.forward.z).normalized;

        //}

        DashStepSound.Play();
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

            //walkDistance is a world scale distance
            //walkDirection * walkDistance is a world scale displacement vector
            


        Vector3 WalkDelta = walkDirection * walkDistance;




        Vector3 finalPositionWorld = Room.transform.position + WalkDelta;
        Vector3 finalPositionLocal;
        if (Room.transform.parent != null)
        {
            finalPositionLocal = Room.transform.parent.InverseTransformPoint(finalPositionWorld);
        } else
        {
            finalPositionLocal = finalPositionWorld;
        }


        //StartCoroutine(WalkAndBlur(Room.transform.position + WalkDelta ));

        //StartCoroutine(WalkAndBlur2(finalPositionLocal, finalPositionWorld));
        StartCoroutine(WalkAndBlur3(walkDirection));
        im.gm.DepleteGripDiscrete(GripDepletion);
        HasStepped = true;

    }
        
    void FixedUpdate()
    {

    }

    // Update is called once per frame
    void Update () {

    }



    IEnumerator WalkAndBlur3(Vector3 walkDirectionWorld)
    {

        //FallingFilter.enabled = true;

        float elapsedTime = 0;
        float x = 0;
        float EffectiveStepSize = StepSize * (1 / StepTime);

        while (elapsedTime < StepTime)
        {
            elapsedTime += Time.deltaTime;
            

            RaycastHit hit; 
            if (Physics.Raycast(Player.transform.position, walkDirectionWorld, out hit, .1f))
            {
                Vignette.intensity = 0.0f;
                yield break;

            }
            else
            {
                x = (elapsedTime / StepTime) * Mathf.PI * 2f; // x goes from 0 to 2pi

                Vector3 newLocation = Room.transform.position + walkDirectionWorld * EffectiveStepSize * Time.deltaTime;// * (Mathf.Cos(x) + 1);

                /* TODO , this is the start of the code to let you walk on inclines... it's trickier than I thought
                //perform raycast to get height you're walking on
                RaycastHit hit2;
                if (Physics.Raycast(new Vector3(newLocation.x, newLocation.y + 1, newLocation.x), Vector3.down, out hit2, 2f))
                {
                    newLocation = new Vector3(newLocation.x, hit2.point.y + , newLocation.z);
                }
                */

                Room.transform.position = newLocation;
                Vignette.intensity = Mathf.Lerp(0, MaxVignette, Mathf.Sin(elapsedTime / StepTime * Mathf.PI));
                //bo.blurSize = Mathf.Lerp(0, 1, Mathf.Sin(elapsedTime / StepTime * Mathf.PI));
            }




        
                yield return null;
          }



     }





    IEnumerator WalkAndBlur2(Vector3 finalPositionLocal, Vector3 finalPositionWorld)
    {

        //FallingFilter.enabled = true;

        float elapsedTime = 0;


        Vector3 startPositionLocal = Room.transform.localPosition;
        Vector3 startPositionWorld = Room.transform.position;

        Vector3 walkingDirectionLocal = (finalPositionLocal- startPositionLocal).normalized;


        


        float x = 0;

        while (elapsedTime < StepTime)
        {
            elapsedTime += Time.deltaTime;

            //the robust way to do this might be to calculate a global position BASED on an update to local position
            // err or set the players global position to be a specific (global) spot calculated in local space

            if (UseTrigStep)
            {
                Vector3 newPosition;
                //use trig functions Sin(0) -> Sin (pi)
                // some bearing vector normalized times a small step size 
                x = (elapsedTime / StepTime) * Mathf.PI / 2 - Mathf.PI / 4.0f;

                if (Room.transform.parent == null)
                {
                    newPosition = Vector3.Lerp(startPositionWorld, finalPositionWorld, (Mathf.Tan(x) / 2 + 0.5f));
                } else
                {
                    newPosition = Vector3.Lerp(startPositionLocal, finalPositionLocal, (Mathf.Tan(x) / 2 + 0.5f));

                }

                //TODO: this has the problem that you'll walk super slowly towards a wall... but I can fix that later. Solution is to make StepTime a function of walking distance

                // if while doing this, we detect that the parent changes, then we fall back on world position
                //longer term, we are going to want to make this recalculate new local values in case you step from one sticky to another, but that would be prematuer to do now



                Room.transform.localPosition = newPosition;
                Vignette.intensity = Mathf.Lerp(0, MaxVignette, Mathf.Sin(elapsedTime / StepTime * Mathf.PI));

                //Room.transform.position += walkingDirection * Mathf.Sin((elapsedTime / StepTime) * Mathf.PI);


                Room.velocity = Vector3.zero;
                yield return null;
            }
            else
            {
                finalPositionLocal.x = finalPositionLocal.x * Room.transform.lossyScale.x;
                finalPositionLocal.y = finalPositionLocal.y * Room.transform.lossyScale.y;
                finalPositionLocal.z = finalPositionLocal.z * Room.transform.lossyScale.z;

                Room.transform.localPosition =  finalPositionLocal;
                yield break;
            }


        }

    }



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

            //the robust way to do this might be to calculate a global position BASED on an update to local position
            // err or set the players global position to be a specific (global) spot calculated in local space

            if (UseTrigStep) { 
                //use trig functions Sin(0) -> Sin (pi)
                // some bearing vector normalized times a small step size 
                x = (elapsedTime / StepTime) * Mathf.PI/2 - Mathf.PI / 4.0f;

                //TODO: this has the problem that you'll walk super slowly towards a wall... but I can fix that later. Solution is to make StepTime a function of walking distance
                Vector3 newPosition = Vector3.Lerp(startPosition, finalPosition, (Mathf.Tan(x) / 2 + 0.5f));
                Room.transform.position = newPosition;
                Vignette.intensity = Mathf.Lerp(0, MaxVignette, Mathf.Sin(elapsedTime / StepTime * Mathf.PI));

                //Room.transform.position += walkingDirection * Mathf.Sin((elapsedTime / StepTime) * Mathf.PI);


                Room.velocity = Vector3.zero;
                yield return null;
            } else
            {
                x = (elapsedTime / StepTime);
                Room.transform.position = Vector3.Lerp(startPosition, finalPosition, x);
            }


        }

    }


}
