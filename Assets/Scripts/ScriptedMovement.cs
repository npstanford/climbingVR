using UnityEngine;
using System.Collections;

public class ScriptedMovement : MonoBehaviour {

    public Transform TargetA;
    public Transform TargetB;
    public GameObject MovingObject;
    public float ObjectSpeed;
    public float ObjectPause;

    private bool MovingTowardA;
    //public bool Moving = true;


    // Use this for initialization
    void Start()
    {
        MovingTowardA = true;
        TargetA.GetComponent<MeshRenderer>().enabled = false;
        TargetB.GetComponent<MeshRenderer>().enabled = false;

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        StartCoroutine("MoveObject");

    }

    IEnumerator MoveObject()
    {

        {
            
            if ((MovingObject.transform.position - TargetA.position).magnitude < .001)
            {
                MovingTowardA = false;
                yield return new WaitForSecondsRealtime(ObjectPause);
            }
            else if ((MovingObject.transform.position - TargetB.position).magnitude < .001)
            {
                MovingTowardA = true;
                yield return new WaitForSecondsRealtime(ObjectPause);
            }

            float[] stuff = {
                    Mathf.Sqrt((MovingObject.transform.position - TargetA.position).magnitude),
                    Mathf.Sqrt((MovingObject.transform.position - TargetB.position).magnitude),
                    1.0f
                    };
            float effectiveSpeed = Mathf.Min(stuff);
            effectiveSpeed = ObjectSpeed * effectiveSpeed;

            if (MovingTowardA)
            {
                MovingObject.transform.position = (MovingObject.transform.position + (TargetA.position - MovingObject.transform.position).normalized * Time.deltaTime * effectiveSpeed);
            }
            else
            {
                MovingObject.transform.position = (MovingObject.transform.position + (TargetB.position - MovingObject.transform.position).normalized * Time.deltaTime * effectiveSpeed);
            }


            yield return null;

        }
    }

    public void StartMovement()
    {
        ObjectSpeed = 1f;
    }

    public void PauseMovement()
    {
        Debug.Log("Stop movement");
        ObjectSpeed = 0f;
    }

}
