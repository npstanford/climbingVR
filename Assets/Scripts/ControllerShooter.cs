using UnityEngine;
using System.Collections;

public class ControllerShooter : MonoBehaviour {

    public ControllerState Controller;
    public Transform BulletOrigin;
    public GameObject Bullet;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	    if(Controller.device.GetPressDown(SteamVR_Controller.ButtonMask.Trigger) && !Controller.canGrip)
        {
            Shoot();
        }


    }

    void Shoot()
    {
        Controller.device.TriggerHapticPulse();
        GameObject bullet = GameObject.Instantiate(Bullet, BulletOrigin.position, BulletOrigin.rotation) as GameObject;
        bullet.GetComponent<Rigidbody>().velocity = 10f * BulletOrigin.transform.forward;
    }


}
