using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.ImageEffects;

public class Walk : MonoBehaviour {

    public float StepSize = 1.0f;
    private bool Stepped = false;
    public float StepTime = 0.1f;
    public float InitialStepVelocity;
    public GameController gc;    
    


    public GameObject Player;
    public Rigidbody Room;

    void Start () {
        Stepped = false;
        GameObject go = GameObject.FindGameObjectWithTag("GameController");
        gc = go.GetComponent<GameController>();
    }

    public void Step(SteamVR_TrackedObject controller) {
        if (gc.PlayerIsTouchingGround)
        {
            Vector3 walkDirection = new Vector3(controller.transform.forward.x, 0, controller.transform.forward.z).normalized;
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

            StartCoroutine(WalkAndBlur(Room.transform.position + walkDirection * walkDistance));

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
    

        Vector3 startPosition = Room.transform.position;

        Vector3 walkingDirection = (finalPosition - startPosition).normalized;


        while (elapsedTime < StepTime)
        {
            elapsedTime += Time.deltaTime;

            //use trig functions Sin(0) -> Sin (pi)
            // some bearing vector normalized times a small step size 
            Room.transform.position += walkingDirection * Mathf.Sin((elapsedTime / StepTime) * Mathf.PI);


            Room.velocity = Vector3.zero;
            //BlinkMask.color = Color.Lerp(BlinkMask.color, Color.clear, elapsedTime / StepTime);
            yield return null;

        }

        //BlinkMask.enabled = false;

        //FallingFilter.enabled = false;
    }

}
