using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoldenBallWatcher : MonoBehaviour
{
    private GoldenBall[] gbArray;
    public GameObject Door;
    public Image DimMask;
    public GameObject NoReturnArea;
    public GameObject DarkRoomLocation;
    public float distance;
    public ScriptedMovement GripTools;

    private bool AllBallsCollected = false;

    // Use this for initialization
    void Start()
    {
       gbArray = FindObjectsOfType<GoldenBall>();


    }

    // Update is called once per frame
    void Update()
    {

    }

    public void CheckWinState()
    {
        int i = 0;
        int totalBalls = gbArray.Length;
        foreach (GoldenBall gb in gbArray)
        {
            if (gb.Attached)
            {

                i += 1;
            }
        }


        if (i == 2)
        {
            GripTools.EnableMovement = true;
        } else if (i == totalBalls)
        {
            //so if all balls are attached
            Win();
        }
    }

    public void Win()
    {
        Door.SetActive(false);
        AllBallsCollected = true;
    }

    private void OnTriggerStay(Collider other)
    {
        if (AllBallsCollected && other.CompareTag("Player"))
        {
            distance = (other.transform.position - NoReturnArea.transform.position).magnitude;
            DimMask.color = Color.Lerp(Color.clear, Color.black, Mathf.Max(20 - distance + 4, 0) / 20);

            if (distance < 3)
            {
                other.transform.position = DarkRoomLocation.transform.position;
                ControllerState[] controllers = FindObjectsOfType<ControllerState>();
                foreach (ControllerState cs in controllers)
                {
                    cs.gameObject.SetActive(false);
                }

                InputManager im = FindObjectOfType<InputManager>();
                im.gameObject.SetActive(false);
            }
        }



    }

    private void OnTriggerExit(Collider other)
    {
        DimMask.color = Color.clear;   
    }
}