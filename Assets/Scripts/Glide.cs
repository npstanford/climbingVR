using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Glide : MonoBehaviour
{

    public GameObject GliderModel;
    public GripTool _GripTool;
    //public BoxCollider OverheadCollider;
    //public GameObject OverheadCollider; // the invisible box collider over the players head
    public Rigidbody Room;
    public float MaxGlideSpeed;
    public float BankSpeed;
    public float GlideSpeed;
    public float GlideFallSpeed;
    public Vector3 GliderOffset;
    public bool IsGliding;
    public float GripDepletion;
    public ColliderManager cm;
    public float DragCoefficient;
    private Vector3 GlideVelocity;
    public bool HasGlided;
    public AudioSource OpenGliderSound;
    public AudioSource CloseGliderSound;
    public AudioSource GliderSoarSound;
    public AudioSource RushingAirSound;


    // private MeshRenderer _gliderRenderer;
    private ColliderManager Body;


    // Use this for initialization
    void Start()
    {
        //_gliderRenderer = GliderModel.GetComponent<MeshRenderer>();
        //_gliderRenderer.enabled = false;
        GliderModel.SetActive(false);
        IsGliding = false;
        Body = Room.GetComponent<ColliderManager>();
        HasGlided = false;
    }

    // Update is called once per frame
    void Update()
    {

    }
    /*
    public void StartGliding(ControllerState controller, bool PlayerIsTouchingGround, bool HandIsOverHead)
    {
        GliderModel.SetActive(true);


        if ((!PlayerIsTouchingGround || Body.WindVelocity != Vector3.zero) && HandIsOverHead)
        {
            IsGliding = true;

            Vector3 glideVector = controller.transform.right;
            glideVector = glideVector.normalized; // glideVector is not yaw. It is the direction of the nose

            int forwardOrBackward = 1; //1 represents forward
            if (controller.transform.right.y > 0)
            {
                forwardOrBackward = -1;
            }

            //this is the idea to basically smooth the transition from forward to backward
            glideVector.y = Mathf.Pow(glideVector.y, 2);

            if(glideVector.y < .1f)
            {
                glideVector = Vector3.zero;
            }

            glideVector = glideVector.normalized;

            //float rollAngle = Vector3.Angle(new Vector3(controller.transform.forward.x, 0.0f, controller.transform.forward.z) , controller.transform.forward);
            Vector3 rollDirection = new Vector3(0.0f, 0.0f, controller.transform.up.z).normalized;
            //rollDirection.z = Mathf.Pow(rollDirection.z, 2);
            rollDirection = rollDirection.normalized;


            glideVector = -rollDirection + (glideVector * forwardOrBackward);
            //glideVector = glideVector * forwardOrBackward;
            glideVector = glideVector.normalized;


            Vector3 glideVelocity = glideVector * GlideSpeed + Room.transform.up * (-1.0f) * GlideFallSpeed;

            glideVelocity.y = Mathf.Min(GlideFallSpeed * -0.6f, glideVelocity.y);

            glideVelocity += Body.WindVelocity;


            Room.position += glideVelocity * Time.deltaTime;
        }
    }
    */

    /*
public void StartGliding(ControllerState controller, bool PlayerIsTouchingGround, bool HandIsOverHead)
{
    GliderModel.SetActive(true);


    if ((!PlayerIsTouchingGround || Body.WindVelocity != Vector3.zero) && HandIsOverHead )
    {
        IsGliding = true;

        Vector3 glideVector = controller.transform.right;
        glideVector = glideVector.normalized; // glideVector is not yaw. It is the direction of the nose


        float rollAngle = Vector3.Angle(new Vector3(controller.transform.forward.x, 0.0f, controller.transform.forward.z)
            , controller.transform.forward);


        int upOrDownRoll = 1;

        if (controller.transform.forward.y > 0)
        {
            upOrDownRoll = -1;
        }


            glideVector = Quaternion.AngleAxis(rollAngle* upOrDownRoll, Vector3.up) * glideVector;



        Vector3 glideVelocity = glideVector * GlideSpeed + Room.transform.up * (-1.0f) * GlideFallSpeed;

        glideVelocity.y = Mathf.Min(GlideFallSpeed*-0.6f, glideVelocity.y);

        glideVelocity += Body.WindVelocity;

        RaycastHit hit;
        if(Physics.Raycast(cm.displayCube.transform.position, glideVelocity, out hit, .2f))
        {
            InteractionAttributes ia = hit.collider.gameObject.GetComponent<InteractionAttributes>();
            if (ia!=null)
            {
                if(ia.CanClimb || ia.IsGround)
                {
                    glideVelocity = Vector3.down * GlideFallSpeed;
                }
            }
        }

        Room.position += glideVelocity * Time.deltaTime;
       } else // note this is hacky... should probably refactor so there is a "show glider" method that input manager calls when buttons pressed, and a start gliding method
    {
        IsGliding = false;
    }

}
*/

    public void StartGliding(ControllerState controller, bool PlayerIsTouchingGround, bool HandIsOverHead)
    {
        if (!GliderModel.activeInHierarchy)
        {
            GliderModel.SetActive(true);
            _GripTool.HideHook();
            OpenGliderSound.Play();
        }

        if ((!PlayerIsTouchingGround || Body.WindVelocity != Vector3.zero) && HandIsOverHead)
        {
            IsGliding = true;
            HasGlided = true;
            if (!GliderSoarSound.isPlaying)
            {
                GliderSoarSound.Play();
                RushingAirSound.Play();
            }
            Room.useGravity = false;
            Room.velocity = Vector3.zero;
            float magnitude = GlideSpeed;
            Vector3 force = forceFromRightVector(controller.transform.right);
            Vector3 forceup = forceFromUpVector(controller.transform.up);
            force += forceup;
            if (Mathf.Abs(forceup.y) > .92f)
            {
                force = Vector3.zero;
            }


            float rollAngle = Vector3.Angle(new Vector3(controller.transform.forward.x, 0.0f, controller.transform.forward.z)
    , controller.transform.forward);
            rollAngle = rollAngle < .1 ? 0 : rollAngle;


            int upOrDownRoll = 1;

            if (controller.transform.forward.y > 0)
            {
                upOrDownRoll = -1;
            }


            Quaternion rollRotation = Quaternion.AngleAxis(rollAngle * upOrDownRoll, Vector3.up);

            Vector3 glideAcceleration = magnitude * force;
            glideAcceleration.y = 0.0f;
            GlideVelocity += rollRotation*glideAcceleration * Time.deltaTime;

            if (GlideVelocity.magnitude > MaxGlideSpeed)
            {
                GlideVelocity = GlideVelocity.normalized * MaxGlideSpeed;
            }



            Vector3 kiteDrag = controller.transform.up.normalized;
            float drag = Vector3.Dot(GlideVelocity.normalized, kiteDrag);
            //if (drag > 0.5f)
            //{
            drag = Mathf.Max(drag, 0.2f);
            float dragPercent = DragCoefficient * drag;
                GlideVelocity -= GlideVelocity * dragPercent * Time.deltaTime;
            //}

            




            //gravity should be greatest when the glider surface is perpendicular to ground (100%) and least when it is parallel to ground (10%)
            Vector3 up = forceFromUpVector(controller.transform.up);

            Vector3 EffectiveFallVelocity = -Vector3.up * (Mathf.Max(0f, (1 - up.y)) * .9f + .1f) * GlideFallSpeed;

            Vector3 totalVelocity;
            // Wind does not add momentum, so we add wind velocity separately.
            if (Body.WindVelocity.magnitude > 0)
            {
                totalVelocity = .2f * GlideVelocity + Body.WindVelocity + EffectiveFallVelocity;

            }
            else
            {
                totalVelocity = GlideVelocity + Body.WindVelocity + EffectiveFallVelocity;

            }

            // Stop velocity if we hit something.
            RaycastHit hit;
            if (Physics.Raycast(cm.displayCube.transform.position, totalVelocity.normalized, out hit, .2f))
            {
                InteractionAttributes ia = hit.collider.gameObject.GetComponent<InteractionAttributes>();
                if (ia != null)
                {
                    if (ia.CanClimb || ia.IsGround)
                    {

                        totalVelocity = Vector3.down * EffectiveFallVelocity.magnitude;
                    }
                }
            }



            Room.transform.position += totalVelocity * Time.deltaTime;
            Debug.Log("Room position: " + Room.position);


        }
        else // note this is hacky... should probably refactor so there is a "show glider" method that input manager calls when buttons pressed, and a start gliding method
        {
            IsGliding = false;
            GlideVelocity = Vector3.zero;
        }
    }

    public void StopGliding(bool PlayerIsStunned = false)
    {
        //_gliderRenderer.enabled = false;
        GliderSoarSound.Stop();
        OpenGliderSound.Stop();
        RushingAirSound.Stop();
        CloseGliderSound.Play();
        GliderModel.SetActive(false);
        if (PlayerIsStunned) { _GripTool.HideHook(); } 
        else { _GripTool.ShowHook(); }
        Room.useGravity = true;
        Room.velocity = new Vector3(Room.velocity.x*0.2f, Mathf.Min(Room.velocity.y, 0), Room.velocity.z*0.2f);
        IsGliding = false;
    }


    public static float rollFromForwardVector(Vector3 forward)
    {
        Vector3 forwardNormalized = forward.normalized;
        return forwardNormalized.y;
    }

    /*
	 * Get the glider 'pitch' from the y-component of the HTC Vive 'right' vector.
	 * 
	 * pitch < 0.0 is down.
	 * pitch = 0.0 is flat.
	 * pitch > 0.0 is up.
	 * 
	 * Returns a value in the range [-1.0 to 1.0].
	 */
    public static float pitchFromRightVector(Vector3 right)
    {
        Vector3 rightNormalized = right.normalized;
        return rightNormalized.y;
    }

    /*
	 * Get rotation force from pitch and roll.
	 *
	 * rotation < 0.0 is to the left.
	 * rotation = 0 is no rotation.
	 * rotation > 0.0 is to the right.
	 *
	 * Returns a value in the range [-1.0 to 1.0].
	 */
    public static float rotationFromPitchAndRoll(float pitch, float roll)
    {
        return pitch * roll;
    }

    /*
	 * Get the force from the HTC Vive 'right' vector.
	 */
    public static Vector3 forceFromRightVector(Vector3 right)
    {
        // Initialize force with the direction the kite is pointing.
        Vector3 force = right.normalized;

        // If pitch is negative (down) then we want to go forward.
        // If pitch is positive (up) then we want to reverse direction.
        float pitch = pitchFromRightVector(right);
        if (pitch > 0.0f)
        {
            force = -force;
        }

        return force;
    }

    /*
	 * Get the force from the HTC Vive 'up' vector.
	 *
	 * This provides lift to the side when the kite rolls.
	 */
    public static Vector3 forceFromUpVector(Vector3 up)
    {
        Vector3 force = -up.normalized;
        return force;
    }


}
