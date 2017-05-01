using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Glide : MonoBehaviour
{

    public GameObject GliderModel;
    //public BoxCollider OverheadCollider;
    //public GameObject OverheadCollider; // the invisible box collider over the players head
    public Rigidbody Room;
    public float GlideSpeed;
    public float GlideFallSpeed;
    public Vector3 GliderOffset;
    public bool IsGliding;
    public float GripDepletion;
    public ColliderManager cm;
    private Vector3 GlideVelocity;

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
    }

    public void StartGliding(ControllerState controller, bool PlayerIsTouchingGround, bool HandIsOverHead)
    {
        GliderModel.SetActive(true);


        if ((!PlayerIsTouchingGround || Body.WindVelocity != Vector3.zero) && HandIsOverHead )
        {
            IsGliding = true;

			float magnitude = GlideSpeed;
			float mass = 3.0f;
			Vector3 glideAcceleration = magnitude * forceFromRightVector (controller.transform.right) / mass;
			GlideVelocity += glideAcceleration * Time.deltaTime;

			// Stop velocity if we hit something.
            RaycastHit hit;
			if(Physics.Raycast(cm.displayCube.transform.position, GlideVelocity, out hit, .2f))
            {
                InteractionAttributes ia = hit.collider.gameObject.GetComponent<InteractionAttributes>();
                if (ia!=null)
                {
                    if(ia.CanClimb || ia.IsGround)
                    {
						GlideVelocity = Vector3.down * GlideFallSpeed;
                    }
                }
            }

			// Wind does not add momentum, so we add wind velocity separately.
			Vector3 totalVelocity = GlideVelocity + Body.WindVelocity;

			Room.position += totalVelocity * Time.deltaTime;
        } else // note this is hacky... should probably refactor so there is a "show glider" method that input manager calls when buttons pressed, and a start gliding method
        {
            IsGliding = false;
        }
    }
  

    public void StopGliding()
    {
        //_gliderRenderer.enabled = false;
        GliderModel.SetActive(false);
        //Room.useGravity = true;
        Room.velocity = new Vector3(Room.velocity.x*0.2f, Mathf.Min(Room.velocity.y, 0), Room.velocity.z*0.2f);
        IsGliding = false;
    }

	/*
	 * Get the glider 'roll' from the y-component of the HTC Vive 'forward' vector.
	 * 
	 * roll < 0.0 is to the right.
	 * roll = 0.0 is flat.
	 * roll > 0.0 is to the left.
	 *
	 * Returns a value in the range [-1.0 to 1.0].
	 */
	public static float rollFromForwardVector (Vector3 forward)
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
	public static float pitchFromRightVector (Vector3 right)
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
	public static float rotationFromPitchAndRoll (float pitch, float roll)
	{
		return pitch * roll;
	}

	/*
	 * Get the force from the HTC Vive 'right' vector.
	 */
	public static Vector3 forceFromRightVector (Vector3 right) 
	{
		// Initialize force with the direction the kite is pointing.
		Vector3 force = right.normalized;

		// If pitch is negative (down) then we want to go forward.
		// If pitch is positive (up) then we want to reverse direction.
		float pitch = pitchFromRightVector(right);
		if (pitch > 0.0f) {
			force = -force;
		}

		return force;
	}

}
