using UnityEngine;
using UnityEngine.UI; //used for stepping so can remove if that is no longer here
using System.Collections;

public class ControllerState : MonoBehaviour
{


    public SteamVR_Controller.Device device;
    public Rigidbody ObjectToPickUp;
    public Rigidbody Holding;
    private ColliderManager cm;
    public float CurrentChargingRate;
    public AudioSource ChargingShakeNoise;
    public AudioSource ControllerShortCircuitSound;

    //end area of locomotion variables

    [HideInInspector]
    public Vector3 prevPos;
    [HideInInspector]
    public Vector3 curPos;
    [HideInInspector]
    public SteamVR_TrackedObject controller;


    public bool canGrip;
    public bool canPickUp;
    public GameObject GripObject;

    private InputManager im;
    private float WhenToStartChargeNoise;
    private float[] SpeedArrayForCharging;
    private int size = 10;
    private int i;

    // Use this for initialization
    void Start()
    {
        controller = GetComponent<SteamVR_TrackedObject>();
        prevPos = controller.transform.localPosition;
        curPos = controller.transform.localPosition;
        device = SteamVR_Controller.Input((int)controller.index);
        cm = FindObjectOfType<ColliderManager>();
        im = FindObjectOfType<InputManager>();

        SpeedArrayForCharging = new float[size];
        for (int j = 0; j<size; j++)
        {
            SpeedArrayForCharging[j] = 0;
        }

    }

    void Update()
    {
        device = SteamVR_Controller.Input((int)controller.index);
        prevPos = curPos;
        curPos = controller.transform.localPosition;

        if(!canPickUp && Holding!=null)
        {
            Holding = null;
        }

        CurrentChargingRate = UpdateChargingRate();
        PlayChargingNoises(CurrentChargingRate);
    }

    public void ControllerShortCircuit()
    {
        ControllerShortCircuitSound.Play();
    }

    private void PlayChargingNoises(float CurrentChargingRate)
    {
        if (CurrentChargingRate > 3 && !im.run.IsRunning)
        {
            if (WhenToStartChargeNoise == 0)
            {
                WhenToStartChargeNoise = Time.time + .8f;
            }
            else if (Time.time > WhenToStartChargeNoise)
            {


                ChargingShakeNoise.volume = Mathf.Max(.5f, CurrentChargingRate / 40);
                if (!ChargingShakeNoise.isPlaying)
                {
                    ChargingShakeNoise.Play();
                }

                device.TriggerHapticPulse((ushort)500);
            }

        }
        else
        {
            ChargingShakeNoise.Stop();
            WhenToStartChargeNoise = 0;
        }
    }

    private float UpdateChargingRate()
    {
        //finds the projection of the displacement vector onto the controllers forward axis
        if (im.run.IsRunning)
        {
            SpeedArrayForCharging[i] = 0f;
        }
        else
        {
            Vector3 displacementVector = (curPos - prevPos);
            float displacementMag = displacementVector.magnitude;
            displacementVector = displacementVector.normalized;

            Vector3 forwardAxis = transform.forward.normalized;

            float displacementAlongAxis = Mathf.Abs(Vector3.Dot(displacementVector, forwardAxis));

            SpeedArrayForCharging[i] = (displacementAlongAxis * displacementMag * im.gm.GripShakeRecoverRate) / Time.deltaTime;
        }

        

        float avg = 0;
        for (int j = 0; j<size; j++)
        {
            avg += SpeedArrayForCharging[j];
        }
        //avg = avg / size;
        i += 1;

        if (i >= size) { i = 0; }

        return avg;

    }


    void OnTriggerEnter(Collider other)
    {
        InteractionAttributes ia = other.GetComponent<InteractionAttributes>();
        if (ia != null)
        {
            if (ia.CanClimb)
            {
                canGrip = true;
                GripObject = other.gameObject;
            } else if (ia.CanPickUp)
            {
                canPickUp = true;
                ObjectToPickUp = ia.ObjectToPickUp; //TODO if this is null, then assignt PickUpObject as other
            } else if (ia.HurtsPlayer)
            {
                cm.PlayerHit(other.gameObject);
            }
        }
        


    }

    void OnTriggerExit(Collider other)
    {
        InteractionAttributes ia = other.GetComponent<InteractionAttributes>();
        if (ia != null)
        {
            if (ia.CanClimb)
            {
                canGrip = false;
                GripObject = null;
            }
            else if (ia.CanPickUp) // THIS IS THE PROBLEM! THIS IS A HUGE BUG. Some objects switch from pickup==true to pickup==false on their own
            {
                canPickUp = false;
                ObjectToPickUp = null;
            }
        }
    }



}

