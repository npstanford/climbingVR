using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GripTool : MonoBehaviour {

    public ControllerState controller;
    public GameObject hook;
    public GameObject trigger;
    public GameObject hookStart;
    public GameObject touchpad;
    public GameObject hookshotTouchpad;
    public AudioSource ClickSound;

    public GameObject gripL;
    public GameObject gripR;


    private Vector3 hookStartPosition;
    private Quaternion hookStartRotation;
    private Vector3 hookActivatedPosition;
    private Quaternion hookActivatedRotation;
    private Vector3 triggerStartPosition;
    private Vector3 triggerActivatedPosition;
    private Vector3 touchpadStartPosition;
    private Vector3 touchpadActivatedPosition;
    private Vector3 gripStartScale;
    private Vector3 gripActivatedScale;
    private bool HooksVisible;

    private MeshRenderer[] mrs;

    private Vector2 triggerPress;

    // Use this for initialization
    void Start () {
        hookActivatedPosition = Vector3.zero;
        hookActivatedRotation = Quaternion.identity;
        HooksVisible = false;


        triggerStartPosition = trigger.transform.localPosition;
        triggerActivatedPosition = triggerStartPosition;
        triggerActivatedPosition.x += .008f;

        touchpadStartPosition = touchpad.transform.localPosition;
        touchpadActivatedPosition = touchpadStartPosition;
        touchpadActivatedPosition.x -= .002f;

        gripStartScale = gripL.transform.localScale;
        gripActivatedScale = gripStartScale;
        gripActivatedScale.z = 0.0005f;
        gripActivatedScale.y -= .001f;
        gripActivatedScale.x -= .001f;

        triggerPress = Vector2.zero;
        mrs = hook.GetComponentsInChildren<MeshRenderer>();

        trigger.GetComponent<MeshRenderer>().enabled = false;
        gripL.GetComponent<MeshRenderer>().enabled = false;
        gripR.GetComponent<MeshRenderer>().enabled = false;
        hookshotTouchpad.GetComponent<MeshRenderer>().enabled = false;

        HideHook();

    }

    // Update is called once per frame
    void Update () {
        if (controller.device.GetTouch(SteamVR_Controller.ButtonMask.Trigger)){

            triggerPress = controller.device.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger);

        } else
        {
            triggerPress = Vector2.zero;
        }

        if (controller.device.GetPressDown(SteamVR_Controller.ButtonMask.Trigger)&& HooksVisible)
        {
            ClickSound.Play();
        }

            hook.transform.localPosition = Vector3.Lerp(hookStart.transform.localPosition, hookActivatedPosition, triggerPress.x);
        hook.transform.localRotation = Quaternion.Lerp(hookStart.transform.localRotation, hookActivatedRotation, triggerPress.x);

        trigger.transform.localPosition = Vector3.Lerp(triggerStartPosition, triggerActivatedPosition, triggerPress.x);


        touchpad.transform.localPosition = (controller.device.GetPress(SteamVR_Controller.ButtonMask.Touchpad)) 
            ? touchpadActivatedPosition : touchpadStartPosition;

        if (controller.device.GetPress(SteamVR_Controller.ButtonMask.Grip))
        {
            gripL.transform.localScale = gripActivatedScale;
            gripR.transform.localScale = gripActivatedScale;
        }
        else
        {
            gripL.transform.localScale = gripStartScale;
            gripR.transform.localScale = gripStartScale;
        }


    }



    public void HideHook()
    {
        HooksVisible = false;
        foreach (MeshRenderer mr in mrs)
        {
            mr.enabled = false;
        }
    }

    public void ShowHook()
    {
        HooksVisible = true;
        foreach (MeshRenderer mr in mrs)
        {
            mr.enabled = true;
        }
    }



    public void DisplayClimbingComponents()
    {
        ShowHook();
        trigger.GetComponent<MeshRenderer>().enabled = true;

    }

    public void DisplayHookshotComponents()
    {
        //hookshotTouchpad.GetComponent<MeshRenderer>().enabled = true;
        gripL.GetComponent<MeshRenderer>().enabled = true;
        gripR.GetComponent<MeshRenderer>().enabled = true;
    }

    public void DisplayGliderComponents ()
    {
        gripL.GetComponent<MeshRenderer>().enabled = true;
        gripR.GetComponent<MeshRenderer>().enabled = true;
    }
}
