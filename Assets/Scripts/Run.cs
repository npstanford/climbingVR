﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.ImageEffects;
public class Run : MonoBehaviour {

    public GameObject Room;
    public GameObject Player;
    public GameObject PlayerHead;
    public float SpeedFactor;
    public float SpeedPower;
    public float MaxSpeed;
    public VignetteAndChromaticAberration EyeBlur;
    public float BlurAmount;
    public AudioSource RushingAirSounds;
    public bool IsRunning;

    public float minHeadSpeedToRun;
    public bool HasRun = false;

    private float TimeOfNextStepSound;
    private InputManager im;

    public void Start()
    {
        HasRun = false;
        IsRunning = false;
        im = FindObjectOfType<InputManager>();

    }

    public void Step(Vector3 direction, float speed)
    {
        float scaledSpeed;
        IsRunning = true;
        HasRun = true;


        scaledSpeed = Mathf.Pow((1 + speed) * SpeedFactor, SpeedPower);
        scaledSpeed = Mathf.Min(scaledSpeed, MaxSpeed);

        Vector3 flatDirection = new Vector3(direction.x, 0f, direction.z).normalized;

        RaycastHit hit;
        if (Physics.Raycast(PlayerHead.transform.position, flatDirection, out hit, im.DistanceFromWalls))
        {
            InteractionAttributes ia = hit.collider.gameObject.GetComponent<InteractionAttributes>();
            if (ia != null)
            {
                if (ia.CanClimb || ia.IsGround)
                {
                    EyeBlur.intensity = 0.0f;
                    return;
                }
            }
        }

        Room.transform.localPosition += flatDirection * scaledSpeed * Time.deltaTime;
        EyeBlur.intensity = Mathf.Min(.5f, (4*EyeBlur.intensity + BlurAmount * (scaledSpeed / MaxSpeed)) / 5);
        //EyeBlur.intensity = BlurAmount * (scaledSpeed / MaxSpeed);

        if (!RushingAirSounds.isPlaying)
        {
            RushingAirSounds.Play();
        }
        else
        {
            //RushingAirSounds.volume = scaledSpeed / MaxSpeed;
        }

    }

    public void Stop()
    {
        EyeBlur.intensity = 0;
        RushingAirSounds.Stop();
        IsRunning = false;
    }
}
