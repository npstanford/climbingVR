using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GripTool : MonoBehaviour {

    public ControllerState controller;
    public GameObject hook;
    public GameObject hookStart;

    private Vector3 hookStartPosition;
    private Quaternion hookStartRotation;
    private Vector3 hookActivatedPosition;
    private Quaternion hookActivatedRotation;
    private MeshRenderer[] mrs;

    private Vector2 triggerPress;

    // Use this for initialization
    void Start () {
        hookActivatedPosition = Vector3.zero;
        hookActivatedRotation = Quaternion.identity;
        triggerPress = Vector2.zero;
        mrs = hook.GetComponentsInChildren<MeshRenderer>();


        //mr = hook.GetComponent<MeshRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
        if (controller.device.GetTouch(SteamVR_Controller.ButtonMask.Trigger)){

            triggerPress = controller.device.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger);

        } else
        {
            triggerPress = Vector2.zero;
        }



        hook.transform.localPosition = Vector3.Lerp(hookStart.transform.localPosition, hookActivatedPosition, triggerPress.x);
        hook.transform.localRotation = Quaternion.Lerp(hookStart.transform.localRotation, hookActivatedRotation, triggerPress.x);
    }



    public void HideHook()
    {
        foreach (MeshRenderer mr in mrs)
        {
            mr.enabled = false;
        }
    }

    public void ShowHook()
    {
        foreach (MeshRenderer mr in mrs)
        {
            mr.enabled = true;
        }
    }
}
