using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Walk : MonoBehaviour {

    public float StepSize = 1.0f;
    private bool Stepped = false;
    public Image BlinkMask;
    public float flashSpeed = .1f;                               // The speed the damageImage will fade at.
    public Color flashColor = new Color(0f, 0f, 0f, 1f); // The colour the damageImage is set to, 
                                                         // Use this for initialization


    public GameObject Player;
    public Rigidbody Room;

    void Start () {
        Stepped = false;
    }

    public void Step(SteamVR_TrackedObject controller) {
        Vector3 walkDirection = new Vector3(controller.transform.forward.x, 0, controller.transform.forward.z).normalized;
        float walkDistance;
        //trim walk so it doesn't go through a wall

        RaycastHit hit; //may need to add layer mask that ignores controllers and some other things
        if (Physics.Raycast(Player.transform.position, walkDirection, out hit, StepSize))
        //if (Physics.Raycast(Body.transform.position, walkDirection, out hit, 10*StepSize))
        {
            float d = Vector3.Distance(hit.point, Player.transform.position);
            if (d < StepSize){
                walkDistance = d;
            } else
            {
                walkDistance = StepSize;
            }

        }
        else
        {
            walkDistance = StepSize;

        }

        Room.MovePosition(Room.transform.position + walkDirection * walkDistance);
        Stepped = true;
        //TODO: Add logic to not walk through walls

    }

    void FixedUpdate()
    {

    }

    // Update is called once per frame
    void Update () {

        if (Stepped)
        {
            BlinkMask.color = flashColor;
            Stepped = false;
        }
        else
        {
            BlinkMask.color = Color.Lerp(BlinkMask.color, Color.clear, flashSpeed * Time.deltaTime);
        }

        //should be equivalent to Vector3.projectOnPlane(controller.transform.forward, Vector3.up);

    }
}
