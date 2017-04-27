using UnityEngine;
using System.Collections;

public class ScriptedMovement : MonoBehaviour {

    //public Transform TargetA;
    //public Transform TargetB;
    public GameObject MovingObject;
    public float ObjectSpeed;
    public float ObjectPause;
    public bool EnableMovement;

    public Transform[] Targets;

    //private bool MovingTowardA;
    private int _targetIndex;
    //public bool Moving = true;


    // Use this for initialization
    void Start()
    {

        foreach (Transform t in Targets) {
            t.GetComponent<MeshRenderer>().enabled = false;

            //t.gameObject.SetActive(false);
        }

        _targetIndex = 0;

        /*
        IEnumerator coroutineMoveObject;
        coroutineMoveObject = MoveObject();

        StartCoroutine(coroutineMoveObject);
        */
        StartCoroutine("MoveObject");

        /*
        MovingTowardA = true;
        TargetA.GetComponent<MeshRenderer>().enabled = false;
        TargetB.GetComponent<MeshRenderer>().enabled = false;
        */

    }

    // Update is called once per frame
    void FixedUpdate()
    {


    }

    IEnumerator MoveObject()
    {
        while (true) {
            while (EnableMovement)
            {
                if ((MovingObject.transform.position - Targets[_targetIndex].position).magnitude < .005)
                {
                    _targetIndex = (_targetIndex + 1) % Targets.Length;
                    yield return new WaitForSecondsRealtime(ObjectPause);
                }


                float deceleration = Mathf.Min(Mathf.Sqrt((MovingObject.transform.position - Targets[_targetIndex].position).magnitude), 1.0f);


                Vector3 moveDirection = (Targets[_targetIndex].position - MovingObject.transform.position).normalized;
                MovingObject.transform.position += moveDirection * ObjectSpeed * deceleration * Time.deltaTime;

                yield return new WaitForEndOfFrame();
                /*
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

            */
            }
            yield return new WaitForEndOfFrame();
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
