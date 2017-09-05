using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingGoldenBall : MonoBehaviour {

    public GoldenBall gb;
    public float minAcceleration;
    public float maxAcceleration;
    public float minTimeToChange;
    public float maxTimeToChange;
    public bool FlyingEnabled;

    private Bounds bounds;
    private Vector3 ballAcceleration;
    private Rigidbody gbRb;
    private Transform originalParent;

	// Use this for initialization
	void Start () {
        FlyingEnabled = true;
        Collider c = this.GetComponent<Collider>();
        bounds = c.bounds;
        gbRb = gb.GetComponent<Rigidbody>();
        ballAcceleration = NewAccelerationVector();
        originalParent = gb.transform.parent;
        
        StartCoroutine("AccelerationManager");
        
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        //tODO, better behavior is that the ball flies around the player until becomes attached
        if (gb.transform.parent != originalParent) { GameObject.Destroy(this.gameObject); }

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
            secondsTillNextAccelerationChange = Random.Range(minTimeToChange, maxTimeToChange);
            yield return new WaitForSeconds(secondsTillNextAccelerationChange);
        }

    }
}
