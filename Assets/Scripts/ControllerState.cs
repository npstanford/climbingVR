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
    public AudioSource ChargingSounds;
    public AudioSource ControllerShortCircuitSound;
    public AudioSource ChargingSuccess;
    public AudioSource ErrorSound;
    public ParticleSystem InteractionCircle;


    //end area of locomotion variables

    [HideInInspector]
    public Vector3 prevPos;
    [HideInInspector]
    public Vector3 curPos;
    [HideInInspector]
    public SteamVR_TrackedObject controller;
   [HideInInspector]
    public Vector3 prevPosWall;
   [HideInInspector]
    public Vector3 curPosWall;




    public bool canGrip;
    public bool canPickUp;
    public GameObject GripObject;

    private GripMeter gm;
    private InputManager im;
    private float WhenToStartCharging;
    private float[] SpeedArrayForCharging;
    private int size = 10;
    private float prevGMStrength;
    private float curGMStrength;
    private int i;
    private ParticleSystem.Particle[] particles;

    // Use this for initialization
    void Start()
    {
        controller = GetComponent<SteamVR_TrackedObject>();
        prevPos = controller.transform.localPosition;
        curPos = controller.transform.localPosition;
        device = SteamVR_Controller.Input((int)controller.index);
        cm = FindObjectOfType<ColliderManager>();
        im = FindObjectOfType<InputManager>();
        gm = FindObjectOfType<GripMeter>();
        particles = new ParticleSystem.Particle[1000];
        if (gm.EnableGripStrength)
        {
            SpeedArrayForCharging = new float[size];
            for (int j = 0; j < size; j++)
            {
                SpeedArrayForCharging[j] = 0;
            }
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

        if (gm.EnableGripStrength)
        {
            CurrentChargingRate = UpdateChargingRate();
            PlayChargingNoises(CurrentChargingRate);
            prevGMStrength = curGMStrength;
            curGMStrength = gm.RemainingGrip;
        }



    }

    public void ControllerShortCircuit()
    {
        ControllerShortCircuitSound.Play();
    }

    public void ControllerError()
    {
        ErrorSound.Play();
    }

    private void PlayChargingNoises(float CurrentChargingRate)
    {
        if (gm.EnableGripStrength)
        {
            //play if charging is sufficient, player isn't doing other things, and grip meter < 0
            if (CurrentChargingRate > 5 && curGMStrength < gm.MaxGrip
                && !im.run.IsRunning && !im.climb.IsClimbing && !im.glide.IsGliding)
            {
                ChargingSounds.volume = Mathf.Max(.5f, CurrentChargingRate / 40);
                if (!ChargingSounds.isPlaying)
                {
                    ChargingSounds.Play();
                }

                device.TriggerHapticPulse((ushort)1000);

            }
            else if (curGMStrength > 0 && prevGMStrength < 0)
            {
                ChargingSuccess.Play();
            }
            else
            {
                ChargingSounds.Stop();
            }
        }
    }

    private float UpdateChargingRate()
    {
        if (gm.EnableGripStrength)
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

                //SpeedArrayForCharging[i] = (displacementAlongAxis * displacementMag * im.gm.GripShakeRecoverRate) / Time.deltaTime;
                SpeedArrayForCharging[i] = (displacementMag * im.gm.GripShakeRecoverRate) / Time.deltaTime;
            }



            float avg = 0;
            for (int j = 0; j < size; j++)
            {
                avg += SpeedArrayForCharging[j];
            }
            //avg = avg / size;
            i += 1;

            if (i >= size) { i = 0; }

            if (avg > 5)
            {
                if (WhenToStartCharging == 0)
                {
                    WhenToStartCharging = Time.time + .8f;
                }
                else if (Time.time > WhenToStartCharging)
                {
                    return avg; // so only return a value when we've measured a value above the threshold for longer than .8s
                }
                else
                {
                    return 0;
                }


            }
            else
            {
                WhenToStartCharging = 0;

            }
            return 0;
        } else
        {
            return 0;
        }
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

    private void OnTriggerStay(Collider other)
    {
        InteractionAttributes ia = other.GetComponent<InteractionAttributes>();

        if (ia != null)
        {
            if (ia.CanClimb || ia.CanPickUp)
            {
                /*
                Vector3 interactionCircleLocation = other.ClosestPointOnBounds(this.transform.position);
                InteractionCircle.SetActive(true);
                InteractionCircle.transform.position = interactionCircleLocation;
                */

                Ray contactDirection = new Ray(this.transform.position, transform.forward);
                RaycastHit hit;
                if (other.Raycast(contactDirection, out hit, 1f)) {
                    InteractionCircle.Play();
                    InteractionCircle.transform.position = hit.point;
                    InteractionCircle.transform.localPosition += transform.InverseTransformDirection(hit.normal).normalized * .01f;
                    InteractionCircle.transform.rotation = Quaternion.LookRotation(hit.normal);

                    //experimental: trying to have  climbing update based on where griphook intersects the wall
                    if (ia.CanClimb)
                    {
                        prevPosWall = curPosWall;
                        curPosWall = transform.InverseTransformPoint(InteractionCircle.transform.position);
                    }


                }

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
                curPosWall = Vector3.zero;
                prevPosWall = Vector3.zero;
                Debug.Log("Exited climb object");
            }
            else if (ia.CanPickUp) // THIS IS THE PROBLEM! THIS IS A HUGE BUG. Some objects switch from pickup==true to pickup==false on their own
            {
                canPickUp = false;
                ObjectToPickUp = null;
            }
            InteractionCircle.Stop();

   
            
            int numParticles = InteractionCircle.GetParticles(particles);

            for (int i = 0; i<numParticles; i++)
            {
                particles[i].remainingLifetime = 0f;
            }

            InteractionCircle.SetParticles(particles, numParticles);
        }
    }


}

