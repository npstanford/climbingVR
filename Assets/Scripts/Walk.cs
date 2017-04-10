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
    private GameController gc;
    public VignetteAndChromaticAberration Vignette;
    public float MaxVignette;  


    public GameObject Player;
    public Rigidbody Room;

    void Start () {
        Stepped = false;
        GameObject go = GameObject.FindGameObjectWithTag("GameController");
        gc = go.GetComponent<GameController>();
    }

    public void Step(SteamVR_TrackedObject controller) {
        if (gc.PlayerIsTouchingGround)
        {   //note commented out the below to try to get the running mechanic to work

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

            StartCoroutine(WalkAndBlur(Room.transform.localPosition + walkDirection * walkDistance));

        }




    }

    void FixedUpdate()
    {

    }

    // Update is called once per frame
    void Update () {

    }

    IEnumerator WalkAndBlur(Vector3 finalPosition)
    {
        
        //FallingFilter.enabled = true;

        float elapsedTime = 0;
    

        Vector3 startPosition = Room.transform.localPosition;
    
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
                Room.transform.localPosition = newPosition;
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
