using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingGoldenBall : MonoBehaviour {

    public GoldenBall gb;
    public float minAcceleration;
    public float maxAcceleration;


    private Bounds bounds;
    private Vector3 ballAcceleration;
    private Rigidbody gbRb;

	// Use this for initialization
	void Start () {
        Collider c = this.GetComponent<Collider>();
        bounds = c.bounds;
        gbRb = gb.GetComponent<Rigidbody>();
        ballAcceleration = NewAccelerationVector();
        StartCoroutine("AccelerationManager");
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        gbRb.AddForce(ballAcceleration, ForceMode.Acceleration);
	}

    public void OnTriggerExit(Collider other)
    {
        //hmmm how do we not make this go crazy if I've grabbed it
        if (other.gameObject == gb)
        {
            ballAcceleration = NewAccelerationVector();
        }
    }

    public Vector3 NewAccelerationVector()
    {
        float accMagnitude = Random.Range(minAcceleration, maxAcceleration);
        Vector3 targetPoint = new Vector3(
             Random.Range(-bounds.extents.x, bounds.extents.x),
             Random.Range(-bounds.extents.y, bounds.extents.y),
             Random.Range(-bounds.extents.z, bounds.extents.z)
            );

        targetPoint = bounds.center + targetPoint;

        return (targetPoint - gb.transform.position).normalized * accMagnitude;

    }

    private IEnumerator AccelerationManager()
    {
        float secondsTillNextAccelerationChange;

        while (true)
        {
            ballAcceleration = NewAccelerationVector();
            secondsTillNextAccelerationChange = Random.Range(3f, 15f);
            yield return new WaitForSeconds(secondsTillNextAccelerationChange);
        }

    }
}
