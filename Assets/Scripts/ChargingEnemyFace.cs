using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargingEnemyFace : MonoBehaviour
{

    public ChargingEnemy ce;
    public GameObject pivot;
    public float faceRotateSpeed;
    private bool trackPlayerIsRunning;

    public Vector3 PlayerDirection;
    public GameObject Player;
    private Quaternion rot;


    public void Start()
    {
        IEnumerator trackPlayer = TrackPlayer();
        StartCoroutine(trackPlayer);
    }


    public void Update()
    {
        if (ce.attack && !ce.IsStunned  && ce.state != ChargingEnemy.GTState.Charging)
        {
            trackPlayerIsRunning = true;
            /*
            float step = faceRotateSpeed * Time.deltaTime;
            pivot.transform.localRotation = Quaternion.RotateTowards(pivot.transform.localRotation, rot, step);
            //pivot.transform.eulerAngles = new Vector3(pivot.transform.rotation.eulerAngles.x,
            //pivot.transform.rotation.eulerAngles.y, pivot.transform.rotation.eulerAngles.z);

            pivot.transform.localEulerAngles = new Vector3(Mathf.Clamp(pivot.transform.localRotation.eulerAngles.x, -15, 15),
            Mathf.Clamp(pivot.transform.localRotation.eulerAngles.y, -15, 15), Mathf.Clamp(pivot.transform.localRotation.eulerAngles.z, -15, 15));
            */
        }
        else
        {
            trackPlayerIsRunning = false;
        }

        if (ce.state == ChargingEnemy.GTState.Charging)
        {
            // if charging, lock the face straight
            pivot.transform.localEulerAngles = new Vector3(7.8f, 0f, 0f);
        }
    }


    private IEnumerator TrackPlayer()
    {
        while (true)
        {
            if (trackPlayerIsRunning)
            {
                UpdateTarget();
                //Quaternion rot = Quaternion.LookRotation(playerDirection);
            }
            yield return new WaitForSeconds(.1f);
        }
    }

    private void UpdateTarget()
    {

        //PlayerDirection = (pivot.transform.InverseTransformDirection(Player.transform.position) - pivot.transform.position);

        //rot = Quaternion.LookRotation(PlayerDirection);

        pivot.transform.LookAt(Player.transform);

        float newY;
        newY = pivot.transform.localEulerAngles.y;
        //convert this into something from -180 to 180;
        if (Mathf.Abs(newY)>180)
        {
            newY = -newY - 180;
        }

        newY = Mathf.Clamp(newY, -15f, 15f);

        //newY = (newY < 0 || newY > 180) ? Mathf.Clamp(newY , -15, 0) : Mathf.Clamp(newY, 0, 15);



        pivot.transform.localEulerAngles = new Vector3(Mathf.Clamp(pivot.transform.localRotation.eulerAngles.x, -15, 15),
            newY, 0);

    }


}