using UnityEngine;
using System.Collections;

public class Hookshot : MonoBehaviour
{
    public GrappleCollider Grapple;
    public GameObject GrappleOffset;
    public GameObject LaserSight;
    public float ShootingAngle = 40.0f;
    private GameObject _grappleOrigin;

    public float GrappleSpeed = 1.0f;
    public float GrappleLength = 15.0f;
    private GameObject grappleTarget;

    // Use this for initialization
    void Start()
    {
        GrappleOffset = new GameObject();
        GrappleOffset.transform.position = new Vector3(0.0f, -0.075f, 0.05f);

        _grappleOrigin = new GameObject("GrappleOrigin");
        _grappleOrigin.transform.localPosition = Vector3.zero;

        LaserSight.GetComponent<MeshRenderer>().enabled = false;

         grappleTarget = new GameObject("GrappleTarget");

    }

    // Update is called once per frame
    void Update()
    {

        

    }

    public void Scan(ControllerState controller)
    {
        LayerMask layerMask = (1 << 16); // layer mask against "grapple" layer
        layerMask += (1 << 2); //ignore raycast layer 
        layerMask += (1 << 9); //ignore the player's body
        layerMask += (1 << 8); //ignore the controllers
        layerMask = ~layerMask;
        RaycastHit hit;
        if (Physics.Raycast(controller.controller.transform.position, controller.controller.transform.forward, out hit, GrappleLength, layerMask))
        {
            LaserSight.transform.position = hit.point;
            LaserSight.transform.localScale = new Vector3(.06f,  .06f, .001f) * Mathf.Max(1.0f, hit.distance / 3.0f);
            LaserSight.transform.rotation = Quaternion.LookRotation(hit.normal);
            LaserSight.GetComponent<MeshRenderer>().enabled = true;

        } else
        {
            LaserSight.GetComponent<MeshRenderer>().enabled = false;

        }
    }

    public void StopScan()
    {
        LaserSight.GetComponent<MeshRenderer>().enabled = false;
    }

    public void Shoot(ControllerState controller)
    {
        if (!Grapple.HookshotFired)
        {
            _grappleOrigin.transform.parent = controller.controller.transform;
            _grappleOrigin.transform.localPosition = Vector3.zero;
            _grappleOrigin.transform.localRotation = Quaternion.identity;
            _grappleOrigin.transform.Rotate(Vector3.right * ShootingAngle);
            _grappleOrigin.transform.localPosition += GrappleOffset.transform.position;



            Grapple.transform.position = _grappleOrigin.transform.position;
            Grapple.transform.rotation = _grappleOrigin.transform.rotation;

            Grapple.transform.parent = controller.controller.transform;

            //if lasertarget is on something, use that as target else use the other way

            //GameObject grappleTarget = new GameObject("GrappleTarget");

            if (LaserSight.GetComponent<MeshRenderer>().enabled)
            {
                grappleTarget.transform.position = LaserSight.transform.position;
            }
            else
            {

                grappleTarget.transform.parent = controller.controller.transform;
                grappleTarget.transform.localPosition = Grapple.transform.localPosition;
                grappleTarget.transform.localRotation = Grapple.transform.localRotation;
                Vector3 targetVector = grappleTarget.transform.localPosition;
                targetVector.z += GrappleLength;
                grappleTarget.transform.localPosition = targetVector;
            }


            Grapple.Shoot(grappleTarget, controller, GrappleLength, GrappleSpeed);


        }

    }


}
