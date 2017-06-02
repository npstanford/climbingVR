using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldenBall : MonoBehaviour {
    private Vector3 startPosition;
    public bool Attached = false;
    private GoldenBallWatcher gbw;

	// Use this for initialization
	void Start () {
        startPosition = transform.position;
        gbw = FindObjectOfType<GoldenBallWatcher>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Attach(Vector3 attachPosition)
    {


        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb!=null) {
            rb.isKinematic = true;
            rb.useGravity = false;
        }


        IEnumerator MoveCoroutine = MoveToPodium(attachPosition);
        //StartCoroutine(MoveCoroutine);
        StartCoroutine("RandomRotation");
        transform.parent = null;
        transform.position = attachPosition;
        Attached = true;
        gbw.CheckWinState();
        //TODO do some cool rotation
    }

    /*
    public void Detach()
    {
        Attached = false;
    }
    */
    private void OnTriggerEnter(Collider other)
    {
        Teleporter t = other.GetComponent<Teleporter>();
        if (t != null)
        {
            transform.position = startPosition;
        }
    }


    IEnumerator RandomRotation()
    {
        float rotX = Random.Range(30, 180);
        float rotY = Random.Range(30, 180);
        float rotZ = Random.Range(30, 180);

        //i'M TIRED SO FUCK IT. We need to disable CanPickUp, but if we do, then our hands don't realize we've let go of it...
        yield return new WaitForSeconds(.3f);

        InteractionAttributes ia = GetComponent<InteractionAttributes>();

        if (ia != null)
        {
            ia.CanPickUp = false;
        }


        while (true)
        {

            transform.localEulerAngles = new Vector3(transform.localEulerAngles.x + rotX * Time.deltaTime,
                transform.localEulerAngles.y + rotY * Time.deltaTime, transform.localEulerAngles.z + rotZ * Time.deltaTime);
            yield return null;
        }
    }



    IEnumerator MoveToPodium(Vector3 destination)
    {
        while ((transform.position - destination).magnitude > .01)
        {
            Vector3 direction = destination - transform.position;
            transform.position += direction.normalized * 1f * Time.deltaTime;
            yield return null;
        }
        InteractionAttributes ia = GetComponent<InteractionAttributes>();
        if (ia != null)
        {
            ia.CanPickUp = false; //maybe try moving this to the end of the coroutine if this works
        }
    }

}
